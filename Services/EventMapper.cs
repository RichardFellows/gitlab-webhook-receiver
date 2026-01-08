using gitlab_webhook_receiver.Models.Events;
using gitlab_webhook_receiver.Models.Webhooks;

namespace gitlab_webhook_receiver.Services;

public class EventMapper
{
    public List<object> MapPipelineWebhook(PipelineWebhook webhook)
    {
        var events = new List<object>();
        var attrs = webhook.ObjectAttributes;
        var status = attrs.Status.ToLower();

        switch (status)
        {
            case "running":
            case "pending":
                events.Add(new PipelineStarted(
                    PipelineId: attrs.Id,
                    ProjectId: webhook.Project.Id,
                    ProjectName: webhook.Project.Name,
                    Ref: attrs.Ref,
                    Sha: attrs.Sha,
                    Status: attrs.Status,
                    StartedAt: attrs.CreatedAt ?? DateTime.UtcNow,
                    Source: attrs.Source,
                    Username: webhook.User.Username,
                    CommitMessage: webhook.Commit?.Message
                ));
                break;

            case "success":
                events.Add(new PipelineCompleted(
                    PipelineId: attrs.Id,
                    ProjectId: webhook.Project.Id,
                    ProjectName: webhook.Project.Name,
                    Ref: attrs.Ref,
                    Sha: attrs.Sha,
                    Status: attrs.Status,
                    CompletedAt: attrs.FinishedAt ?? DateTime.UtcNow,
                    DurationSeconds: attrs.Duration,
                    QueuedDurationSeconds: attrs.QueuedDuration,
                    Username: webhook.User.Username,
                    Stages: attrs.Stages
                ));
                break;

            case "failed":
                events.Add(new PipelineFailed(
                    PipelineId: attrs.Id,
                    ProjectId: webhook.Project.Id,
                    ProjectName: webhook.Project.Name,
                    Ref: attrs.Ref,
                    Sha: attrs.Sha,
                    FailedAt: attrs.FinishedAt ?? DateTime.UtcNow,
                    DurationSeconds: attrs.Duration,
                    Username: webhook.User.Username,
                    DetailedStatus: attrs.DetailedStatus
                ));
                break;

            case "canceled":
            case "cancelled":
                events.Add(new PipelineCanceled(
                    PipelineId: attrs.Id,
                    ProjectId: webhook.Project.Id,
                    ProjectName: webhook.Project.Name,
                    Ref: attrs.Ref,
                    CanceledAt: attrs.FinishedAt ?? DateTime.UtcNow,
                    Username: webhook.User.Username
                ));
                break;
        }

        return events;
    }

    public List<object> MapJobWebhook(JobWebhook webhook)
    {
        var events = new List<object>();
        var status = webhook.BuildStatus.ToLower();
        var environment = webhook.Environment?.Name;
        var isDeployment = !string.IsNullOrEmpty(environment);

        switch (status)
        {
            case "running":
            case "pending":
                if (isDeployment && webhook.Environment != null)
                {
                    events.Add(new DeploymentStarted(
                        JobId: webhook.BuildId,
                        PipelineId: webhook.PipelineId,
                        ProjectId: webhook.ProjectId,
                        ProjectName: webhook.ProjectName,
                        Environment: environment!,
                        DeploymentTier: webhook.Environment.DeploymentTier ?? "unknown",
                        Ref: webhook.Ref ?? string.Empty,
                        StartedAt: webhook.BuildStartedAt ?? DateTime.UtcNow,
                        Username: webhook.User?.Username
                    ));
                }
                else
                {
                    events.Add(new JobStarted(
                        JobId: webhook.BuildId,
                        PipelineId: webhook.PipelineId,
                        ProjectId: webhook.ProjectId,
                        ProjectName: webhook.ProjectName,
                        JobName: webhook.BuildName,
                        Stage: webhook.BuildStage,
                        Ref: webhook.Ref ?? string.Empty,
                        StartedAt: webhook.BuildStartedAt ?? DateTime.UtcNow,
                        Environment: environment,
                        Username: webhook.User?.Username
                    ));
                }
                break;

            case "success":
                if (isDeployment && webhook.Environment != null)
                {
                    events.Add(new DeploymentCompleted(
                        JobId: webhook.BuildId,
                        PipelineId: webhook.PipelineId,
                        ProjectId: webhook.ProjectId,
                        ProjectName: webhook.ProjectName,
                        Environment: environment!,
                        DeploymentTier: webhook.Environment.DeploymentTier ?? "unknown",
                        Ref: webhook.Ref ?? string.Empty,
                        CompletedAt: webhook.BuildFinishedAt ?? DateTime.UtcNow,
                        DurationSeconds: webhook.BuildDuration,
                        Username: webhook.User?.Username
                    ));
                }
                else
                {
                    events.Add(new JobCompleted(
                        JobId: webhook.BuildId,
                        PipelineId: webhook.PipelineId,
                        ProjectId: webhook.ProjectId,
                        ProjectName: webhook.ProjectName,
                        JobName: webhook.BuildName,
                        Stage: webhook.BuildStage,
                        Status: webhook.BuildStatus,
                        CompletedAt: webhook.BuildFinishedAt ?? DateTime.UtcNow,
                        DurationSeconds: webhook.BuildDuration,
                        AllowFailure: webhook.BuildAllowFailure,
                        Environment: environment,
                        Username: webhook.User?.Username
                    ));
                }
                break;

            case "failed":
                if (isDeployment && webhook.Environment != null)
                {
                    events.Add(new DeploymentFailed(
                        JobId: webhook.BuildId,
                        PipelineId: webhook.PipelineId,
                        ProjectId: webhook.ProjectId,
                        ProjectName: webhook.ProjectName,
                        Environment: environment!,
                        DeploymentTier: webhook.Environment.DeploymentTier ?? "unknown",
                        Ref: webhook.Ref ?? string.Empty,
                        FailedAt: webhook.BuildFinishedAt ?? DateTime.UtcNow,
                        DurationSeconds: webhook.BuildDuration,
                        FailureReason: webhook.BuildFailureReason,
                        Username: webhook.User?.Username
                    ));
                }
                else
                {
                    events.Add(new JobFailed(
                        JobId: webhook.BuildId,
                        PipelineId: webhook.PipelineId,
                        ProjectId: webhook.ProjectId,
                        ProjectName: webhook.ProjectName,
                        JobName: webhook.BuildName,
                        Stage: webhook.BuildStage,
                        FailedAt: webhook.BuildFinishedAt ?? DateTime.UtcNow,
                        DurationSeconds: webhook.BuildDuration,
                        FailureReason: webhook.BuildFailureReason,
                        AllowFailure: webhook.BuildAllowFailure,
                        Environment: environment,
                        Username: webhook.User?.Username
                    ));
                }
                break;
        }

        return events;
    }
}
