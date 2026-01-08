using gitlab_webhook_receiver.Commands;
using gitlab_webhook_receiver.Services;
using Marten;

namespace gitlab_webhook_receiver.Handlers;

public class PipelineWebhookHandler
{
    private readonly EventMapper _mapper;
    private readonly ILogger<PipelineWebhookHandler> _logger;

    public PipelineWebhookHandler(
        EventMapper mapper,
        ILogger<PipelineWebhookHandler> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(
        RecordPipelineWebhook command,
        IDocumentSession session)
    {
        var pipelineId = command.Webhook.ObjectAttributes.Id;
        var streamId = $"pipeline-{pipelineId}";

        _logger.LogInformation(
            "Processing pipeline webhook for pipeline {PipelineId}, status: {Status}",
            pipelineId,
            command.Webhook.ObjectAttributes.Status);

        var events = _mapper.MapPipelineWebhook(command.Webhook);

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
                "No events mapped for pipeline {PipelineId}, status: {Status}",
                pipelineId,
                command.Webhook.ObjectAttributes.Status);
        }

        // SaveChanges is handled automatically by Wolverine middleware
    }
}
