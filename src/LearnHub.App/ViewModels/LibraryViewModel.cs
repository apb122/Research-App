using CommunityToolkit.Mvvm.ComponentModel;
using LearnHub.Core.Models;

namespace LearnHub.App.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private List<DownloadRecord> _downloads = new();
}
