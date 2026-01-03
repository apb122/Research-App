using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnHub.Core.Models;
using LearnHub.Core.Services;
using LearnHub.Core.Providers;

namespace LearnHub.App.ViewModels;

public partial class ResearchViewModel : ObservableObject
{
    private readonly SourceDiscoveryService _sourceService;
    private readonly VideoDiscoveryService _videoService;
    private readonly PlanGeneratorService _planService;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private bool _preferReputableDomains = true;

    [ObservableProperty]
    private List<SourceItem> _searchResults = new();

    [ObservableProperty]
    private List<VideoItem> _videoResults = new();

    [ObservableProperty]
    private LearningPlan? _plan;

    [ObservableProperty]
    private bool _isSearching = false;

    [ObservableProperty]
    private bool _isGeneratingPlan = false;

    [ObservableProperty]
    private string _planPrompt = string.Empty;

    [ObservableProperty]
    private SourceItem? _selectedSource;

    [ObservableProperty]
    private VideoItem? _selectedVideo;

    public ResearchViewModel(
        SourceDiscoveryService sourceService,
        VideoDiscoveryService videoService,
        PlanGeneratorService planService)
    {
        _sourceService = sourceService;
        _videoService = videoService;
        _planService = planService;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(Query)) return;

        IsSearching = true;
        try
        {
            var sourceTask = _sourceService.SearchAsync(Query, PreferReputableDomains);
            var videoTask = _videoService.SearchAsync(Query);

            await Task.WhenAll(sourceTask, videoTask);

            SearchResults = sourceTask.Result.ToList();
            VideoResults = videoTask.Result.ToList();
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task GeneratePlanAsync()
    {
        if (string.IsNullOrWhiteSpace(PlanPrompt)) return;

        IsGeneratingPlan = true;
        try
        {
            Plan = await _planService.GeneratePlanAsync(PlanPrompt);
        }
        finally
        {
            IsGeneratingPlan = false;
        }
    }

    [RelayCommand]
    private void OpenSource(SourceItem? source)
    {
        if (source?.Url != null)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = source.Url,
                UseShellExecute = true
            });
        }
    }

    [RelayCommand]
    private void OpenVideo(VideoItem? video)
    {
        if (video?.Url != null)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = video.Url,
                UseShellExecute = true
            });
        }
    }
}

