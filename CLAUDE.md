# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core 10.0 web API designed to receive and process GitLab webhooks using an event-sourcing architecture. The project implements the "Critter Stack" pattern (Wolverine + Marten + PostgreSQL) to reliably capture pipeline and deployment events as immutable event streams.

## Technology Stack

- **Framework**: .NET 10.0
- **Project Type**: ASP.NET Core Web API
- **API Style**: Minimal APIs (no controllers)
- **Message Bus**: Wolverine 4.0 (for command/message handling)
- **Event Store**: Marten 8.0-rc-2 (PostgreSQL-backed event store)
- **Database**: PostgreSQL
- **Configuration**: Uses `appsettings.json` and `appsettings.Development.json`

## Architecture

This application follows an **event-sourced architecture** where GitLab webhooks are transformed into immutable events and stored in PostgreSQL via Marten's event store. Wolverine provides reliable message processing with transactional outbox guarantees.

### Event Flow

1. GitLab sends webhook → `POST /webhooks/gitlab`
2. Endpoint validates `X-Gitlab-Token` header
3. Endpoint creates a Wolverine command (`RecordPipelineWebhook` or `RecordJobWebhook`)
4. Command published to Wolverine local queue
5. Handler processes command and maps webhook to domain events
6. Events appended to Marten event stream
7. Transaction committed atomically (Wolverine + Marten integration)

### Key Design Patterns

- **Event Sourcing**: All webhook data stored as immutable events
- **CQRS**: Commands for writes, queries for reads
- **Transactional Outbox**: Wolverine ensures exactly-once processing
- **Stream-per-Entity**: Each pipeline/job gets its own event stream

## Project Structure

```
/
├── Program.cs                          # Application entry point
├── appsettings.json                    # Configuration (PostgreSQL, webhook secret)
├── test-webhooks.http                  # Test webhook payloads
│
├── Configuration/
│   ├── MartenConfiguration.cs         # Marten event store setup
│   └── WolverineConfiguration.cs      # Message bus configuration
│
├── Endpoints/
│   ├── WebhookEndpoints.cs            # POST /webhooks/gitlab
│   └── QueryEndpoints.cs              # GET endpoints for querying events
│
├── Models/
│   ├── Webhooks/                      # GitLab webhook payload DTOs
│   │   ├── PipelineWebhook.cs        # Pipeline webhook schema
│   │   ├── JobWebhook.cs             # Job/deployment webhook schema
│   │   └── Common/                    # Shared models (Project, User, Commit, Build)
│   └── Events/                        # Marten event types (domain events)
│       ├── PipelineEvents.cs          # PipelineStarted, PipelineCompleted, etc.
│       └── JobEvents.cs               # JobStarted, DeploymentCompleted, etc.
│
├── Commands/
│   ├── RecordPipelineWebhook.cs      # Wolverine command for pipeline webhooks
│   └── RecordJobWebhook.cs           # Wolverine command for job webhooks
│
├── Handlers/
│   ├── PipelineWebhookHandler.cs     # Processes pipeline webhook commands
│   └── JobWebhookHandler.cs          # Processes job webhook commands
│
└── Services/
    ├── WebhookAuthenticationService.cs # Validates X-Gitlab-Token
    └── EventMapper.cs                 # Maps webhooks to domain events
```

## Event Streams

Events are organized into streams using string-based identities:

- **Pipeline streams**: `pipeline-{pipelineId}` (e.g., `pipeline-12345`)
- **Job streams**: `job-{jobId}` (e.g., `job-67890`)

Each stream contains a complete audit trail of all events for that entity.

### Event Types

**Pipeline Events** (`Models/Events/PipelineEvents.cs`):
- `PipelineStarted` - Pipeline begins (status: running/pending)
- `PipelineCompleted` - Pipeline finishes successfully (status: success)
- `PipelineFailed` - Pipeline fails (status: failed)
- `PipelineCanceled` - Pipeline is canceled

**Job Events** (`Models/Events/JobEvents.cs`):
- `JobStarted` - Job begins
- `JobCompleted` - Job finishes successfully
- `JobFailed` - Job fails with failure reason
- `DeploymentStarted` - Deployment job starts (has environment)
- `DeploymentCompleted` - Deployment succeeds
- `DeploymentFailed` - Deployment fails

## Common Commands

### Build
```bash
dotnet build
```

### Run the Application
```bash
dotnet run
```

The application runs on:
- HTTP: `http://localhost:5045`
- HTTPS: `https://localhost:7282`

### Restore Dependencies
```bash
dotnet restore
```

### Clean Build Artifacts
```bash
dotnet clean
```

## Configuration

### Required Settings

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=gitlab_webhooks;Username=postgres;Password=password"
  },
  "GitLab": {
    "WebhookSecret": "your-secret-token"
  }
}
```

### PostgreSQL Setup

The application requires a PostgreSQL database. You can run PostgreSQL using Docker:

```bash
docker run -d \
  --name gitlab-webhooks-postgres \
  -p 5432:5432 \
  -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=gitlab_webhooks_dev \
  postgres:16
```

Marten will automatically create the schema on first run (tables: `mt_events`, `mt_streams` in `public` schema).

## API Endpoints

### Webhook Receiver
- **POST /webhooks/gitlab** - Receives GitLab webhooks
  - Required headers:
    - `X-Gitlab-Token`: Webhook secret (must match config)
    - `X-Gitlab-Event`: Event type (`Pipeline Hook`, `Job Hook`, `Build Hook`)
  - Returns: `202 Accepted` on success

### Query Endpoints
- **GET /pipelines/{pipelineId}/events** - Get all events for a pipeline
- **GET /jobs/{jobId}/events** - Get all events for a job
- **GET /streams/{streamId}/events** - Get events for any stream
- **GET /health** - Health check endpoint

### Testing with test-webhooks.http

The repository includes `test-webhooks.http` with sample webhook payloads:
- Pipeline started, completed, failed
- Job running, success, failed
- Deployment events (production, staging)

Use VS Code REST Client extension or curl to send test webhooks.

## Development Notes

### Adding New Event Types

1. **Define event record** in `Models/Events/`:
   ```csharp
   public record MyNewEvent(int Id, string Status, DateTime Timestamp);
   ```

2. **Update EventMapper** (`Services/EventMapper.cs`) to create the event based on webhook data

3. Events are automatically registered by Marten (no manual registration needed)

### Adding New Webhook Types

1. **Create webhook model** in `Models/Webhooks/` matching GitLab's JSON schema

2. **Create command** in `Commands/`:
   ```csharp
   public record RecordMyWebhook(MyWebhook Webhook, DateTime ReceivedAt);
   ```

3. **Create handler** in `Handlers/`:
   ```csharp
   public class MyWebhookHandler
   {
       public async Task Handle(RecordMyWebhook command, IDocumentSession session)
       {
           var streamId = $"my-entity-{command.Webhook.Id}";
           var events = MapToEvents(command.Webhook);
           session.Events.Append(streamId, events);
       }
   }
   ```

4. **Update WolverineConfiguration** to route command to queue

5. **Update WebhookEndpoints** to handle the new `X-Gitlab-Event` type

### Wolverine Message Handling

- Handlers automatically participate in transactions (via `Wolverine.Marten` integration)
- No need to call `session.SaveChangesAsync()` - Wolverine handles it
- Failed messages are automatically retried
- Use `IDocumentSession` for writes, `IQuerySession` for reads

### Querying Events

```csharp
// In a handler or endpoint
var events = await session.Events.FetchStreamAsync("pipeline-12345");
```

Or use the query endpoints to retrieve events via HTTP.

### Marten Event Store

Events are stored in PostgreSQL with JSONB columns:
- Table: `mt_events` (contains all events)
- Table: `mt_streams` (stream metadata)
- Schema: `public` (default, configurable in MartenConfiguration.cs)

Query events directly in PostgreSQL:
```sql
SELECT stream_id, type, data, timestamp
FROM mt_events
WHERE stream_id = 'pipeline-12345'
ORDER BY version;
```

## GitLab Integration

### Configuring GitLab Webhook

1. Go to your GitLab project → Settings → Webhooks
2. URL: `https://your-server:7282/webhooks/gitlab`
3. Secret token: (same as `GitLab:WebhookSecret` in config)
4. Trigger events:
   - ✓ Pipeline events
   - ✓ Job events
5. Click "Add webhook"

### Supported Event Types

- **Pipeline Hook** - Pipeline status changes (running, success, failed, canceled)
- **Job Hook** / **Build Hook** - Job status changes, including deployments

## Security Considerations

- **Webhook authentication**: All webhooks must include valid `X-Gitlab-Token` header
- **HTTPS**: Application configured for HTTPS by default
- **PostgreSQL credentials**: Store in environment variables in production
- **Secrets**: Never commit actual secrets to git; use placeholder values

## Troubleshooting

### Webhook rejected (401 Unauthorized)
- Check that `X-Gitlab-Token` header matches `GitLab:WebhookSecret` in configuration

### Events not appearing in database
- Verify PostgreSQL connection string is correct
- Check application logs for Wolverine/Marten errors
- Ensure database exists and is accessible

### Handler not executing
- Check Wolverine logs (set `Wolverine` log level to `Debug`)
- Verify command is registered in `WolverineConfiguration.cs`
- Ensure handler class is public and follows Wolverine conventions

## Key Project Settings

- Nullable reference types are enabled
- Implicit usings are enabled
- Root namespace: `gitlab_webhook_receiver`
- Target framework: `net10.0`

## Dependencies

Key NuGet packages:
- `WolverineFx.Http` (4.0.0) - Wolverine message bus
- `WolverineFx.Marten` (4.0.0) - Wolverine + Marten integration
- `Marten` (8.0.0-rc-2) - Event store
- `Marten.AspNetCore` (8.0.0-rc-2) - ASP.NET Core integration
- `Npgsql` (9.0.2) - PostgreSQL driver
