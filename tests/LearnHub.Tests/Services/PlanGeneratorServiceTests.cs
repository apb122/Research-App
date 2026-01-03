using System;
using System.Threading;
using System.Threading.Tasks;
using LearnHub.Core.Providers;
using LearnHub.Core.Services;
using LearnHub.Infrastructure.Providers;
using Xunit;

namespace LearnHub.Tests.Services;

public class PlanGeneratorServiceTests
{
    [Fact]
    public async Task GeneratesValidPlanFromLocalClient()
    {
        var service = new PlanGeneratorService(new LocalAiClient());
        var plan = await service.GeneratePlanAsync("C# async");

        Assert.False(string.IsNullOrWhiteSpace(plan.PlanTitle));
        Assert.NotEmpty(plan.WeeklySchedule);
    }

    [Fact]
    public async Task ThrowsOnInvalidJson()
    {
        var client = new FailingAiClient();
        var service = new PlanGeneratorService(client);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ValidateAsync("{ }"));
    }

    private class FailingAiClient : IAIClient
    {
        public Task<string> GeneratePlanJsonAsync(string prompt, CancellationToken cancellationToken = default)
        {
            return Task.FromResult("{}");
        }
    }
}
