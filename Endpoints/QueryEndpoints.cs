using Marten;

namespace gitlab_webhook_receiver.Endpoints;

public static class QueryEndpoints
{
    public static IEndpointRouteBuilder MapQueryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/pipelines/{pipelineId}/events", GetPipelineEvents)
            .WithName("GetPipelineEvents")
            .WithOpenApi()
            .Produces<List<object>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/jobs/{jobId}/events", GetJobEvents)
            .WithName("GetJobEvents")
            .WithOpenApi()
            .Produces<List<object>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/streams/{streamId}/events", GetStreamEvents)
            .WithName("GetStreamEvents")
            .WithOpenApi()
            .Produces<List<object>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetPipelineEvents(
        int pipelineId,
        IQuerySession session,
        ILogger<IQuerySession> logger)
    {
        var streamId = $"pipeline-{pipelineId}";
        logger.LogInformation("Fetching events for stream: {StreamId}", streamId);

        var events = await session.Events.FetchStreamAsync(streamId);

        if (events == null || events.Count == 0)
        {
            logger.LogInformation("No events found for stream: {StreamId}", streamId);
            return Results.NotFound($"No events found for pipeline {pipelineId}");
        }

        var eventData = events.Select(e => new
        {
            Id = e.Id,
            StreamId = e.StreamId,
            Version = e.Version,
            Sequence = e.Sequence,
            Timestamp = e.Timestamp,
            EventType = e.EventType.Name,
            Data = e.Data
        }).ToList();

        return Results.Ok(eventData);
    }

    private static async Task<IResult> GetJobEvents(
        int jobId,
        IQuerySession session,
        ILogger<IQuerySession> logger)
    {
        var streamId = $"job-{jobId}";
        logger.LogInformation("Fetching events for stream: {StreamId}", streamId);

        var events = await session.Events.FetchStreamAsync(streamId);

        if (events == null || events.Count == 0)
        {
            logger.LogInformation("No events found for stream: {StreamId}", streamId);
            return Results.NotFound($"No events found for job {jobId}");
        }

        var eventData = events.Select(e => new
        {
            Id = e.Id,
            StreamId = e.StreamId,
            Version = e.Version,
            Sequence = e.Sequence,
            Timestamp = e.Timestamp,
            EventType = e.EventType.Name,
            Data = e.Data
        }).ToList();

        return Results.Ok(eventData);
    }

    private static async Task<IResult> GetStreamEvents(
        string streamId,
        IQuerySession session,
        ILogger<IQuerySession> logger)
    {
        logger.LogInformation("Fetching events for stream: {StreamId}", streamId);

        var events = await session.Events.FetchStreamAsync(streamId);

        if (events == null || events.Count == 0)
        {
            logger.LogInformation("No events found for stream: {StreamId}", streamId);
            return Results.NotFound($"No events found for stream {streamId}");
        }

        var eventData = events.Select(e => new
        {
            Id = e.Id,
            StreamId = e.StreamId,
            Version = e.Version,
            Sequence = e.Sequence,
            Timestamp = e.Timestamp,
            EventType = e.EventType.Name,
            Data = e.Data
        }).ToList();

        return Results.Ok(eventData);
    }
}
