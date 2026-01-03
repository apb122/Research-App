using System.Text.Json;
using LearnHub.Core.Models;
using LearnHub.Core.Providers;

namespace LearnHub.Infrastructure.Providers;

public class LocalAiClient : IAIClient
{
    public Task<string> GeneratePlanJsonAsync(string prompt, CancellationToken cancellationToken = default)
    {
        // Deterministic template to avoid relying on remote LLMs in this sample.
        var demoPlan = new LearningPlan
        {
            PlanTitle = $"Plan for {prompt}",
            Prerequisites = new[] { "Basic computer literacy" },
            Milestones = new[] { "Build a mini project", "Complete quiz" },
            WeeklySchedule = new[]
            {
                new WeeklySchedule
                {
                    WeekLabel = "Week 1",
                    Days = new[]
                    {
                        new PlanDay
                        {
                            DayLabel = "Day 1",
                            Reading = new[]{"Official docs section 1"},
                            Watching = new[]{"Intro video"},
                            PracticeTasks = new[]{"Install SDK", "Hello world"},
                            Checkpoints = new[]{"Share screenshot"}
                        }
                    }
                }
            },
            SpacedRepetitionReminders = new[]{"Review flashcards on Friday"}
        };

        var json = JsonSerializer.Serialize(demoPlan, new JsonSerializerOptions { WriteIndented = true });
        return Task.FromResult(json);
    }
}
