using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnHub.Core.Models;
using LearnHub.Core.Services;

namespace LearnHub.App.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly SourceDiscoveryService _service;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private bool _preferReputableDomains = true;

    [ObservableProperty]
    private List<SourceItem> _results = new();

    public SearchViewModel(SourceDiscoveryService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(Query)) return;
        var items = await _service.SearchAsync(Query, PreferReputableDomains);
        Results = items.ToList();
    }
}
