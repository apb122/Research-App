using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnHub.Infrastructure.Services;

namespace LearnHub.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly YtDlpService _ytDlpService;

    [ObservableProperty]
    private string _ytDlpPath = string.Empty;

    [ObservableProperty]
    private string _detectionMessage = "yt-dlp not detected";

    public SettingsViewModel(YtDlpService ytDlpService)
    {
        _ytDlpService = ytDlpService;
    }

    [RelayCommand]
    private async Task DetectAsync()
    {
        var found = await _ytDlpService.DetectAsync(YtDlpPath);
        DetectionMessage = found ? $"yt-dlp found at {_ytDlpService.ExecutablePath}" : "yt-dlp not found";
    }
}
