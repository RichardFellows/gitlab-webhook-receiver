namespace gitlab_webhook_receiver.Models.Events;

public record JobStarted(
    int JobId,
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string JobName,
    string Stage,
    string Ref,
    DateTime StartedAt,
    string? Environment,
    string? Username
);

public record JobCompleted(
    int JobId,
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string JobName,
    string Stage,
    string Status,
    DateTime CompletedAt,
    double? DurationSeconds,
    bool AllowFailure,
    string? Environment,
    string? Username
);

public record JobFailed(
    int JobId,
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string JobName,
    string Stage,
    DateTime FailedAt,
    double? DurationSeconds,
    string? FailureReason,
    bool AllowFailure,
    string? Environment,
    string? Username
);

public record DeploymentStarted(
    int JobId,
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Environment,
    string DeploymentTier,
    string Ref,
    DateTime StartedAt,
    string? Username
);

public record DeploymentCompleted(
    int JobId,
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Environment,
    string DeploymentTier,
    string Ref,
    DateTime CompletedAt,
    double? DurationSeconds,
    string? Username
);

public record DeploymentFailed(
    int JobId,
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Environment,
    string DeploymentTier,
    string Ref,
    DateTime FailedAt,
    double? DurationSeconds,
    string? FailureReason,
    string? Username
);
