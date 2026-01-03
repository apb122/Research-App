# LearnHub

LearnHub is a Windows 10/11 desktop application built with .NET 8 and WPF. It is an AI-powered learning planner and study hub that helps learners discover trusted sources, browse and filter videos, and generate structured study plans that can be tracked day by day.

## Getting started

1. **Install prerequisites**
   - .NET 8 SDK with Windows desktop workload.
   - SQLite (bundled via Microsoft.Data.Sqlite; no external install required).
   - yt-dlp (optional for downloads). Place it on PATH or set the executable path in Settings.

2. **Restore and build**
   - `dotnet restore`
   - `dotnet build LearnHub.sln`

3. **Run the WPF app**
   - `dotnet run --project src/LearnHub.App`

4. **Configuration**
   - Set a YouTube Data API key (and other providers) in appsettings or a secure secrets store. The sample uses placeholder providers; wire in your preferred APIs inside `src/LearnHub.Infrastructure/Providers`.

## Architecture overview

- **UI (WPF)**: Navigation-first shell with views for Search, Videos, Plan, Library, and Settings. MVVM is powered by CommunityToolkit.Mvvm.
- **Domain (LearnHub.Core)**: Models (`SourceItem`, `VideoItem`, `LearningPlan`, `JobStatus`, `DownloadRecord`) and services (`SourceDiscoveryService`, `VideoDiscoveryService`, `PlanGeneratorService`) plus provider interfaces (`IWebSearchProvider`, `IVideoProvider`, `IAIClient`, `IJobRunner`).
- **Infrastructure**: SQLite persistence via `LearnHubDbContext`, default providers (`DefaultWebSearchProvider`, `DefaultVideoProvider`, `LocalAiClient`), a job runner, yt-dlp integration, and file-based logging.
- **Jobs pipeline**: `JobRunner` tracks queued → running → succeeded/failed with correlation IDs per job.
- **Logging**: `FileLoggerProvider` writes structured log lines under `%LOCALAPPDATA%/LearnHub/logs/learnhub.log`.

## Compliance and safety

- The app **does not bypass paywalls or DRM** and does not include any code for cookies extraction, geo-bypass, or restriction circumvention.
- Downloads are **optional**; users must point to a local yt-dlp binary and should only download when they have rights or when allowed by platform terms. The Settings screen exposes detection and messaging before enabling downloads.
- Web content is treated as untrusted. Plan generation runs through a strict JSON schema validator (`PlanGeneratorService`) to resist prompt-injection and malformed output.
- Prefer linking/embedding over downloading. Discovery uses provider abstractions to swap in official APIs (e.g., YouTube Data API) rather than scraping.

## Database schema

`LearnHubDbContext` defines tables for sources, videos, plans, and download records. EF Core conventions are used; run migrations in a Windows environment with `dotnet ef migrations add InitialCreate` followed by `dotnet ef database update`.

## Tests

Run unit tests with:

```
dotnet test LearnHub.sln
```

Included coverage:
- Plan JSON schema validation
- Web search provider abstraction behavior
- yt-dlp detection helper logic

## Project layout

- `src/LearnHub.App` – WPF shell, views, and view models
- `src/LearnHub.Core` – Domain models and services
- `src/LearnHub.Infrastructure` – Persistence, providers, job runner, logging, yt-dlp integration
- `tests/LearnHub.Tests` – Minimal unit tests

## Security & compliance notes

- No DRM or paywall bypassing is present. Download features rely on a user-supplied yt-dlp binary and should be used only where permitted.
- Providers are designed to prefer official APIs; replace the sample stubs with compliant implementations.
- Plan rendering uses sanitized, structured JSON only; raw web snippets are treated as untrusted input.
