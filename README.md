# GitLab Webhook Receiver

An event-sourced GitLab webhook receiver built with ASP.NET Core 10.0, Wolverine, and Marten.

## Quick Start with Docker Compose

### Prerequisites
- Docker and Docker Compose installed
- Git

### Run the Application

```bash
# Clone the repository (if needed)
git clone <repository-url>
cd gitlab-webhook-receiver

# Start both PostgreSQL and the Web API
docker-compose up -d

# View logs
docker-compose logs -f webapi
```

The application will be available at:
- **HTTP**: http://localhost:5045

### Stop the Application

```bash
docker-compose down

# To also remove the database volume
docker-compose down -v
```

## Architecture

This application implements an **event-sourced architecture** using the "Critter Stack":
- **Wolverine**: Message bus for reliable command processing
- **Marten**: PostgreSQL-backed event store
- **PostgreSQL**: Database for event storage

### How It Works

1. GitLab sends webhook → `POST /webhooks/gitlab`
2. Webhook validated and transformed into Wolverine command
3. Command processed asynchronously by handler
4. Events appended to Marten event stream
5. Transaction committed atomically

## API Endpoints

### Webhook Receiver
- **POST /webhooks/gitlab** - Receives GitLab webhooks
  - Headers:
    - `X-Gitlab-Token`: Secret token (default: `dev-secret-token`)
    - `X-Gitlab-Event`: Event type (`Pipeline Hook`, `Job Hook`)

### Query Endpoints
- **GET /pipelines/{pipelineId}/events** - Get pipeline events
- **GET /jobs/{jobId}/events** - Get job events
- **GET /streams/{streamId}/events** - Get events by stream ID
- **GET /health** - Health check

## Configuration

### Docker Compose Environment Variables

Edit `docker-compose.yml` to customize:

```yaml
environment:
  - ConnectionStrings__PostgreSQL=Host=postgres;Port=5432;Database=gitlab_webhooks;Username=postgres;Password=your_password
  - GitLab__WebhookSecret=your-secret-token
```

### Running Locally (without Docker)

```bash
# Start PostgreSQL
docker run -d -p 5432:5432 \
  -e POSTGRES_DB=gitlab_webhooks_dev \
  -e POSTGRES_PASSWORD=password \
  postgres:16

# Update appsettings.Development.json with connection string

# Run the application
dotnet run
```

## Testing

Use the included test webhook payloads:

```bash
# Using the test-webhooks.http file (VS Code REST Client extension)
# Open test-webhooks.http and click "Send Request"

# Or using curl
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Pipeline Hook" \
  -H "Content-Type: application/json" \
  -d @test-payload.json
```

## GitLab Configuration

1. Go to your GitLab project → **Settings** → **Webhooks**
2. **URL**: `http://your-server:5045/webhooks/gitlab`
3. **Secret Token**: (same as `GitLab__WebhookSecret` in config)
4. **Trigger events**:
   - ✓ Pipeline events
   - ✓ Job events
5. Click **Add webhook**

## Event Types

### Pipeline Events
- `PipelineStarted` - Pipeline begins
- `PipelineCompleted` - Pipeline succeeds
- `PipelineFailed` - Pipeline fails
- `PipelineCanceled` - Pipeline canceled

### Job Events
- `JobStarted` - Job begins
- `JobCompleted` - Job succeeds
- `JobFailed` - Job fails
- `DeploymentStarted` - Deployment begins (with environment)
- `DeploymentCompleted` - Deployment succeeds
- `DeploymentFailed` - Deployment fails

## Querying Events

After receiving webhooks, query the event store:

```bash
# Get all events for pipeline 12345
curl http://localhost:5045/pipelines/12345/events

# Get all events for job 67890
curl http://localhost:5045/jobs/67890/events
```

## Database Access

Connect to PostgreSQL to query events directly:

```bash
# Connect to PostgreSQL container
docker exec -it gitlab-webhooks-postgres psql -U postgres -d gitlab_webhooks

# Query all events
SELECT stream_id, type, data->>'Status' as status, timestamp
FROM mt_events
ORDER BY timestamp DESC
LIMIT 10;

# Query specific stream
SELECT * FROM mt_events WHERE stream_id = 'pipeline-12345' ORDER BY version;
```

## Development

See [CLAUDE.md](CLAUDE.md) for detailed architecture documentation and development guidelines.

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Project Structure
- `Configuration/` - Marten and Wolverine setup
- `Endpoints/` - HTTP endpoints
- `Models/` - Webhook payloads and event types
- `Commands/` - Wolverine commands
- `Handlers/` - Command handlers
- `Services/` - Application services

## Troubleshooting

### Webhook returns 401 Unauthorized
- Check that `X-Gitlab-Token` header matches `GitLab__WebhookSecret` in configuration

### Events not appearing in database
- Check application logs: `docker-compose logs -f webapi`
- Verify PostgreSQL is running: `docker-compose ps`
- Check database connection: `docker exec -it gitlab-webhooks-postgres psql -U postgres -c '\l'`

### Container won't start
- Check logs: `docker-compose logs webapi`
- Verify ports are available: `netstat -an | grep 5045`
- Rebuild containers: `docker-compose up --build`

## License

[Your License Here]

## Contributing

[Your Contributing Guidelines Here]
