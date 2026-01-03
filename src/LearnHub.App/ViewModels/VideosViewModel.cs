using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnHub.Core.Models;
using LearnHub.Core.Services;

namespace LearnHub.App.ViewModels;

public partial class VideosViewModel : ObservableObject
{
    private readonly VideoDiscoveryService _service;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private List<VideoItem> _results = new();

    [ObservableProperty]
    private string _durationFilter = "All";

    public List<string> DurationFilters { get; } = new() { "All", "<10m", "10-30m", "30-60m", "1-2h", ">2h" };

    public VideosViewModel(VideoDiscoveryService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(Query)) return;
        var items = await _service.SearchAsync(Query);
        Results = ApplyDurationFilter(items).ToList();
    }

    partial void OnDurationFilterChanged(string value)
    {
        Results = ApplyDurationFilter(Results).ToList();
    }

    private IEnumerable<VideoItem> ApplyDurationFilter(IEnumerable<VideoItem> items)
    {
        return DurationFilter switch
        {
            "<10m" => items.Where(v => v.Duration.TotalMinutes < 10),
            "10-30m" => items.Where(v => v.Duration.TotalMinutes is >= 10 and < 30),
            "30-60m" => items.Where(v => v.Duration.TotalMinutes is >= 30 and < 60),
            "1-2h" => items.Where(v => v.Duration.TotalMinutes is >= 60 and < 120),
            ">2h" => items.Where(v => v.Duration.TotalMinutes >= 120),
            _ => items
        };
    }
}
