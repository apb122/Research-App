using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using LearnHub.Core.Models;
using LearnHub.Core.Providers;
using Microsoft.Extensions.Logging;

namespace LearnHub.Infrastructure.Providers;

public class DuckDuckGoWebSearchProvider : IWebSearchProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DuckDuckGoWebSearchProvider>? _logger;

    public DuckDuckGoWebSearchProvider(HttpClient? httpClient = null, ILogger<DuckDuckGoWebSearchProvider>? logger = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        _logger = logger;
    }

    public async Task<IReadOnlyList<SourceItem>> SearchWebAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<SourceItem>();

        try
        {
            var encodedQuery = HttpUtility.UrlEncode(query);
            var searchUrl = $"https://html.duckduckgo.com/html/?q={encodedQuery}";

            _logger?.LogInformation("Searching DuckDuckGo for: {Query}", query);

            var response = await _httpClient.GetStringAsync(searchUrl, cancellationToken);
            var results = ParseSearchResults(response, query);

            _logger?.LogInformation("Found {Count} results for query: {Query}", results.Count, query);
            return results;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error searching DuckDuckGo for query: {Query}", query);
            // Return empty results on error rather than throwing
            return Array.Empty<SourceItem>();
        }
    }

    private List<SourceItem> ParseSearchResults(string html, string originalQuery)
    {
        var results = new List<SourceItem>();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // DuckDuckGo HTML search results - try multiple selectors
        var resultNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'result')]")
            ?? doc.DocumentNode.SelectNodes("//div[@class='web-result']")
            ?? doc.DocumentNode.SelectNodes("//div[contains(@class, 'web-result')]")
            ?? doc.DocumentNode.SelectNodes("//div[contains(@class, 'result__body')]")
            ?? doc.DocumentNode.SelectNodes("//div[@class='links_main links_deep result__body']")
            ?? doc.DocumentNode.SelectNodes("//div[contains(@class, 'result-link')]");

        if (resultNodes == null)
        {
            _logger?.LogWarning("Could not find search result nodes in HTML. Trying alternative parsing...");
            // Try to find any links that look like search results
            resultNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'result__a')]");
            if (resultNodes != null)
            {
                // If we found links, create results from them
                foreach (var linkNode in resultNodes.Take(10))
                {
                    try
                    {
                        var title = HttpUtility.HtmlDecode(linkNode.InnerText?.Trim() ?? string.Empty);
                        var url = linkNode.GetAttributeValue("href", string.Empty);
                        
                        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(url))
                            continue;

                        // Clean up DuckDuckGo redirect URLs
                        url = CleanDuckDuckGoUrl(url);
                        if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
                            continue;

                        var uri = new Uri(url);
                        var domain = uri.Host;
                        var credibilityScore = CalculateCredibilityScore(domain);

                        // Try to find snippet nearby
                        var snippet = string.Empty;
                        var parent = linkNode.ParentNode;
                        if (parent != null)
                        {
                            var snippetNode = parent.SelectSingleNode(".//a[contains(@class, 'result__snippet')]")
                                ?? parent.SelectSingleNode(".//div[contains(@class, 'result__snippet')]");
                            snippet = snippetNode != null 
                                ? HttpUtility.HtmlDecode(snippetNode.InnerText?.Trim() ?? string.Empty)
                                : string.Empty;
                        }

                        results.Add(new SourceItem
                        {
                            Title = title,
                            Url = url,
                            Domain = domain,
                            Snippet = snippet,
                            PublishedDate = null,
                            CredibilityScore = credibilityScore
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error parsing link node");
                        continue;
                    }
                }
                return results;
            }
            
            _logger?.LogWarning("Could not find any search results in HTML");
            return results;
        }

        foreach (var node in resultNodes.Take(10)) // Limit to 10 results
        {
            try
            {
                var titleNode = node.SelectSingleNode(".//a[contains(@class, 'result__a')]") 
                    ?? node.SelectSingleNode(".//a[@class='result-link']")
                    ?? node.SelectSingleNode(".//h2/a");

                var snippetNode = node.SelectSingleNode(".//a[contains(@class, 'result__snippet')]")
                    ?? node.SelectSingleNode(".//div[contains(@class, 'result__snippet')]")
                    ?? node.SelectSingleNode(".//span[contains(@class, 'result__snippet')]");

                if (titleNode == null) continue;

                var title = HttpUtility.HtmlDecode(titleNode.InnerText?.Trim() ?? string.Empty);
                var url = titleNode.GetAttributeValue("href", string.Empty);
                
                // Clean up DuckDuckGo redirect URLs
                url = CleanDuckDuckGoUrl(url);

                if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    // Try to extract from onclick or other attributes
                    var onclick = titleNode.GetAttributeValue("onclick", string.Empty);
                    var urlMatch = Regex.Match(onclick, @"'(https?://[^']+)'");
                    if (urlMatch.Success)
                    {
                        url = urlMatch.Groups[1].Value;
                    }
                    else
                    {
                        // Try data attribute
                        url = titleNode.GetAttributeValue("data-url", string.Empty);
                    }
                }

                var snippet = snippetNode != null 
                    ? HttpUtility.HtmlDecode(snippetNode.InnerText?.Trim() ?? string.Empty)
                    : string.Empty;

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(url))
                    continue;

                var uri = new Uri(url);
                var domain = uri.Host;

                // Calculate credibility score based on domain
                var credibilityScore = CalculateCredibilityScore(domain);

                results.Add(new SourceItem
                {
                    Title = title,
                    Url = url,
                    Domain = domain,
                    Snippet = snippet,
                    PublishedDate = null, // DuckDuckGo HTML doesn't provide dates
                    CredibilityScore = credibilityScore
                });
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error parsing search result node");
                continue;
            }
        }

        return results;
    }

    private double CalculateCredibilityScore(string domain)
    {
        // Calculate credibility based on domain characteristics
        var score = 0.5; // Base score

        // Educational domains
        if (domain.EndsWith(".edu") || domain.EndsWith(".ac.uk") || domain.EndsWith(".edu.au"))
            score = 1.0;
        // Government domains
        else if (domain.EndsWith(".gov") || domain.EndsWith(".gov.uk"))
            score = 0.95;
        // Well-known reputable domains
        else if (domain.Contains("wikipedia.org") || 
                 domain.Contains("stackoverflow.com") || 
                 domain.Contains("github.com") ||
                 domain.Contains("microsoft.com") ||
                 domain.Contains("mozilla.org") ||
                 domain.Contains("w3.org"))
            score = 0.9;
        // Documentation sites
        else if (domain.Contains("docs.") || domain.Contains("documentation"))
            score = 0.85;
        // News sites
        else if (domain.Contains("bbc.") || domain.Contains("reuters.") || domain.Contains("ap.org"))
            score = 0.8;
        // Medium, blogs
        else if (domain.Contains("medium.com") || domain.Contains("blog."))
            score = 0.6;

        return Math.Min(1.0, Math.Max(0.0, score));
    }

    private string CleanDuckDuckGoUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        // Handle DuckDuckGo redirect URLs
        if (url.StartsWith("/l/?kh=") || url.StartsWith("/l/"))
        {
            var redirectMatch = Regex.Match(url, @"uddg=([^&]+)");
            if (redirectMatch.Success)
            {
                return HttpUtility.UrlDecode(redirectMatch.Groups[1].Value);
            }
        }

        // Handle relative URLs
        if (url.StartsWith("/"))
        {
            return "https://duckduckgo.com" + url;
        }

        return url;
    }
}

