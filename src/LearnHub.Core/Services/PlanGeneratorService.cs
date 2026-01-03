using System.Text.Json;
using Json.Schema;
using LearnHub.Core.Models;
using LearnHub.Core.Providers;

namespace LearnHub.Core.Services;

public class PlanGeneratorService
{
    private readonly IAIClient _aiClient;
    private readonly JsonSchema _schema;

    public PlanGeneratorService(IAIClient aiClient)
    {
        _aiClient = aiClient;
        _schema = BuildSchema();
    }

    public async Task<LearningPlan> GeneratePlanAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var raw = await _aiClient.GeneratePlanJsonAsync(prompt, cancellationToken);
        var plan = await ValidateAsync(raw, cancellationToken);
        return plan;
    }

    public async Task<LearningPlan> ValidateAsync(string json, CancellationToken cancellationToken = default)
    {
        using var document = JsonDocument.Parse(json);
        var result = _schema.Evaluate(document, new EvaluationOptions { OutputFormat = OutputFormat.List });
        if (!result.IsValid)
        {
            throw new InvalidOperationException("Plan JSON did not satisfy schema: " + string.Join("; ", result.Details.Select(d => d.ToString())));
        }

        await Task.CompletedTask;
        return document.Deserialize<LearningPlan>() ?? throw new InvalidOperationException("Unable to deserialize plan");
    }

    private static JsonSchema BuildSchema()
    {
        return new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .Required("planTitle", "prerequisites", "weeklySchedule", "milestones")
            .Properties(
                ("planTitle", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                ("prerequisites", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String))),
                ("milestones", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String))),
                ("spacedRepetitionReminders", new JsonSchemaBuilder().Type(SchemaValueType.Array | SchemaValueType.Null).Items(new JsonSchemaBuilder().Type(SchemaValueType.String))),
                ("weeklySchedule", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.Object)
                        .Required("weekLabel", "days")
                        .Properties(
                            ("weekLabel", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                            ("days", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(
                                new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Object)
                                    .Required("dayLabel", "reading", "watching", "practiceTasks", "checkpoints")
                                    .Properties(
                                        ("dayLabel", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                        ("reading", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String))),
                                        ("watching", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String))),
                                        ("practiceTasks", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String))),
                                        ("checkpoints", new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String)))
                                    )
                            ))
                        )
                ))
            );
    }
}
