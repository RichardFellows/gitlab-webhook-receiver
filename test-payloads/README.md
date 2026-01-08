# Test Payloads

This directory contains sample GitLab webhook payloads in JSON format for testing with curl or other HTTP clients.

## Files

- **pipeline-running.json** - Pipeline webhook with status "running"
- **pipeline-success.json** - Pipeline webhook with status "success"
- **job-success.json** - Job webhook with successful job
- **deployment-production.json** - Deployment job to production environment

## Usage with curl

### Send a pipeline webhook

```bash
# Pipeline running
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Pipeline Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/pipeline-running.json

# Pipeline success
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Pipeline Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/pipeline-success.json
```

### Send a job webhook

```bash
# Job success
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Job Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/job-success.json

# Deployment to production
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Job Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/deployment-production.json
```

### Query the stored events

```bash
# Get pipeline events
curl http://localhost:5045/pipelines/12345/events

# Get job events
curl http://localhost:5045/jobs/1002/events

# Get deployment events
curl http://localhost:5045/jobs/2001/events
```

## Using with Docker Compose

If running with Docker Compose, use the same commands as above.

## Complete Test Flow

```bash
# 1. Send pipeline started
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Pipeline Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/pipeline-running.json

# 2. Send pipeline completed
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Pipeline Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/pipeline-success.json

# 3. Query the pipeline events (should see 2 events)
curl http://localhost:5045/pipelines/12345/events | jq

# 4. Send deployment webhook
curl -X POST http://localhost:5045/webhooks/gitlab \
  -H "X-Gitlab-Token: dev-secret-token" \
  -H "X-Gitlab-Event: Job Hook" \
  -H "Content-Type: application/json" \
  -d @test-payloads/deployment-production.json

# 5. Query deployment events
curl http://localhost:5045/jobs/2001/events | jq
```

## Notes

- All payloads use the same pipeline ID (12345) and project ID (42) for consistency
- The webhook secret in these examples is `dev-secret-token` (default for development)
- Pipeline ID 12345 will accumulate events as you send multiple webhooks
- Use `jq` for pretty-printing JSON responses (install with `apt install jq` or `brew install jq`)
