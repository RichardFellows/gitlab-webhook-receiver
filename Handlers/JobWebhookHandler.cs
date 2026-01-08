using gitlab_webhook_receiver.Commands;
using gitlab_webhook_receiver.Services;
using Marten;

namespace gitlab_webhook_receiver.Handlers;

public class JobWebhookHandler
{
    private readonly EventMapper _mapper;
    private readonly ILogger<JobWebhookHandler> _logger;

    public JobWebhookHandler(
        EventMapper mapper,
        ILogger<JobWebhookHandler> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(
        RecordJobWebhook command,
        IDocumentSession session)
    {
        var jobId = command.Webhook.BuildId;
        var streamId = $"job-{jobId}";
        var environment = command.Webhook.Environment?.Name;

        _logger.LogInformation(
            "Processing job webhook for job {JobId} ({JobName}), status: {Status}, environment: {Environment}",
            jobId,
            command.Webhook.BuildName,
            command.Webhook.BuildStatus,
            environment ?? "none");

        var events = _mapper.MapJobWebhook(command.Webhook);

        if (events.Count > 0)
        {
            session.Events.Append(streamId, events.ToArray());

            _logger.LogInformation(
                "Appended {EventCount} event(s) to stream {StreamId}",
                events.Count,
                streamId);
        }
        else
        {
            _logger.LogWarning(
                "No events mapped for job {JobId}, status: {Status}",
                jobId,
                command.Webhook.BuildStatus);
        }

        // SaveChanges is handled automatically by Wolverine middleware
    }
}
