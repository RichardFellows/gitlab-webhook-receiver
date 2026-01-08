using gitlab_webhook_receiver.Commands;
using gitlab_webhook_receiver.Models.Webhooks;
using gitlab_webhook_receiver.Services;
using Wolverine;

namespace gitlab_webhook_receiver.Endpoints;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/webhooks/gitlab", HandleGitLabWebhook)
            .WithName("ReceiveGitLabWebhook")
            .WithOpenApi()
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> HandleGitLabWebhook(
        HttpRequest request,
        IMessageBus messageBus,
        WebhookAuthenticationService authService,
        ILogger<WebhookAuthenticationService> logger)
    {
        // Validate webhook token
        var token = request.Headers["X-Gitlab-Token"].FirstOrDefault();
        if (!authService.ValidateToken(token))
        {
            logger.LogWarning("Unauthorized webhook request received");
            return Results.Unauthorized();
        }

        // Get event type from header
        var eventType = request.Headers["X-Gitlab-Event"].FirstOrDefault();
        if (string.IsNullOrEmpty(eventType))
        {
            logger.LogWarning("Webhook request missing X-Gitlab-Event header");
            return Results.BadRequest("Missing X-Gitlab-Event header");
        }

        logger.LogInformation("Received GitLab webhook: {EventType}", eventType);

        try
        {
            var receivedAt = DateTime.UtcNow;

            // Route based on event type
            switch (eventType)
            {
                case "Pipeline Hook":
                    var pipelineWebhook = await request.ReadFromJsonAsync<PipelineWebhook>();
                    if (pipelineWebhook == null)
                    {
                        return Results.BadRequest("Failed to parse pipeline webhook payload");
                    }

                    var pipelineCommand = new RecordPipelineWebhook(pipelineWebhook, receivedAt);
                    await messageBus.PublishAsync(pipelineCommand);

                    logger.LogInformation(
                        "Pipeline webhook queued: Pipeline {PipelineId}, Status: {Status}",
                        pipelineWebhook.ObjectAttributes.Id,
                        pipelineWebhook.ObjectAttributes.Status);
                    break;

                case "Job Hook":
                case "Build Hook":
                    var jobWebhook = await request.ReadFromJsonAsync<JobWebhook>();
                    if (jobWebhook == null)
                    {
                        return Results.BadRequest("Failed to parse job webhook payload");
                    }

                    var jobCommand = new RecordJobWebhook(jobWebhook, receivedAt);
                    await messageBus.PublishAsync(jobCommand);

                    logger.LogInformation(
                        "Job webhook queued: Job {JobId} ({JobName}), Status: {Status}",
                        jobWebhook.BuildId,
                        jobWebhook.BuildName,
                        jobWebhook.BuildStatus);
                    break;

                default:
                    logger.LogWarning("Unsupported event type: {EventType}", eventType);
                    return Results.BadRequest($"Unsupported event type: {eventType}");
            }

            return Results.Accepted();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing webhook for event type: {EventType}", eventType);
            return Results.Problem("Error processing webhook");
        }
    }
}
