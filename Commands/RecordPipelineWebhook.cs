using gitlab_webhook_receiver.Models.Webhooks;

namespace gitlab_webhook_receiver.Commands;

public record RecordPipelineWebhook(
    PipelineWebhook Webhook,
    DateTime ReceivedAt
);
