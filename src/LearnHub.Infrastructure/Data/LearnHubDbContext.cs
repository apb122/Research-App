using LearnHub.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Infrastructure.Data;

public class LearnHubDbContext : DbContext
{
    public DbSet<SourceItem> Sources => Set<SourceItem>();
    public DbSet<VideoItem> Videos => Set<VideoItem>();
    public DbSet<LearningPlan> Plans => Set<LearningPlan>();
    public DbSet<DownloadRecord> Downloads => Set<DownloadRecord>();

    public string DbPath { get; }

    public LearnHubDbContext(DbContextOptions<LearnHubDbContext> options) : base(options)
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        DbPath = Path.Combine(folder, "LearnHub", "learnhub.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SourceItem>().HasKey(x => x.Id);
        modelBuilder.Entity<VideoItem>().HasKey(x => x.VideoId);
        modelBuilder.Entity<DownloadRecord>().HasKey(x => x.Id);

        modelBuilder.Entity<LearningPlan>().HasKey(nameof(LearningPlan.PlanTitle));
        modelBuilder.Entity<LearningPlan>().Property(p => p.PlanTitle);
        modelBuilder.Entity<LearningPlan>().Ignore(p => p.SpacedRepetitionReminders);
        modelBuilder.Entity<LearningPlan>().Ignore(p => p.WeeklySchedule);
        modelBuilder.Entity<LearningPlan>().Ignore(p => p.Prerequisites);
        modelBuilder.Entity<LearningPlan>().Ignore(p => p.Milestones);

        base.OnModelCreating(modelBuilder);
    }
}
