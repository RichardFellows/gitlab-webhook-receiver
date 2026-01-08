using Wolverine;
using gitlab_webhook_receiver.Commands;

namespace gitlab_webhook_receiver.Configuration;

public static class WolverineConfiguration
{
    public static void ConfigureWolverine(this WolverineOptions opts)
    {
        // Configure local queue for webhook processing
        opts.LocalQueue("webhooks")
            .Sequential()  // Process one at a time per stream for ordering
            .MaximumParallelMessages(10);

        // Route commands to the webhooks queue
        opts.PublishMessage<RecordPipelineWebhook>()
            .ToLocalQueue("webhooks");

        opts.PublishMessage<RecordJobWebhook>()
            .ToLocalQueue("webhooks");

        // Configure policies
        opts.Policies.AutoApplyTransactions();

        // Configure durability with Marten
        opts.Policies.UseDurableLocalQueues();
    }
}
