using System;
using System.IO;
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
    private readonly SourceDiscoveryService _sourceService;
    private readonly VideoDiscoveryService _videoService;
    private readonly PlanGeneratorService _planService;
    private readonly YtDlpService _ytDlpService;

    [ObservableProperty]
    private object? _currentView;

    public MainViewModel()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LearnHub", "logs", "learnhub.log");
            builder.AddProvider(new LearnHub.Infrastructure.Logging.FileLoggerProvider(logPath));
        });

        // Initialize services
        _sourceService = new SourceDiscoveryService(new DefaultWebSearchProvider());
        _videoService = new VideoDiscoveryService(new DefaultVideoProvider());
        _planService = new PlanGeneratorService(new LocalAiClient());
        _ytDlpService = new YtDlpService(_loggerFactory.CreateLogger<YtDlpService>());

        // Start with Research view
        NavigateToResearch();
    }

    [RelayCommand]
    private void NavigateToResearch()
    {
        CurrentView = new ResearchView 
        { 
            DataContext = new ResearchViewModel(_sourceService, _videoService, _planService) 
        };
        OnPropertyChanged(nameof(CurrentView));
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = new SettingsView 
        { 
            DataContext = new SettingsViewModel(_ytDlpService) 
        };
        OnPropertyChanged(nameof(CurrentView));
    }
}
