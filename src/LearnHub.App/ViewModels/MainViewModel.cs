using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnHub.App.Views;
using LearnHub.Core.Services;
using LearnHub.Infrastructure.Providers;
using LearnHub.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace LearnHub.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ILoggerFactory _loggerFactory;
    public IReadOnlyList<string> NavigationItems { get; } = new[] { "Search", "Videos", "Plan", "Library", "Settings" };

    [ObservableProperty]
    private string _selectedNavigation;

    [ObservableProperty]
    private object? _currentView;

    public MainViewModel()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LearnHub", "logs", "learnhub.log");
            builder.AddProvider(new LearnHub.Infrastructure.Logging.FileLoggerProvider(logPath));
        });

        _selectedNavigation = NavigationItems.First();
        Navigate(_selectedNavigation);
    }

    partial void OnSelectedNavigationChanged(string value)
    {
        Navigate(value);
    }

    private void Navigate(string destination)
    {
        _currentView = destination switch
        {
            "Search" => new SearchView { DataContext = new SearchViewModel(new SourceDiscoveryService(new DefaultWebSearchProvider())) },
            "Videos" => new VideosView { DataContext = new VideosViewModel(new VideoDiscoveryService(new DefaultVideoProvider())) },
            "Plan" => new PlanView { DataContext = new PlanViewModel(new PlanGeneratorService(new LocalAiClient())) },
            "Library" => new LibraryView { DataContext = new LibraryViewModel() },
            "Settings" => new SettingsView { DataContext = new SettingsViewModel(new YtDlpService(_loggerFactory.CreateLogger<YtDlpService>())) },
            _ => null
        };
        OnPropertyChanged(nameof(CurrentView));
    }
}
