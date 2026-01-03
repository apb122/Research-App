using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnHub.Core.Models;
using LearnHub.Core.Services;

namespace LearnHub.App.ViewModels;

public partial class PlanViewModel : ObservableObject
{
    private readonly PlanGeneratorService _service;

    [ObservableProperty]
    private string _prompt = string.Empty;

    [ObservableProperty]
    private LearningPlan? _plan;

    public PlanViewModel(PlanGeneratorService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task GenerateAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt)) return;
        Plan = await _service.GeneratePlanAsync(Prompt);
    }
}
