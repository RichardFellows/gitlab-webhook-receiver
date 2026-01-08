using gitlab_webhook_receiver.Models.Webhooks;

namespace gitlab_webhook_receiver.Commands;

public record RecordJobWebhook(
    JobWebhook Webhook,
    DateTime ReceivedAt
);
