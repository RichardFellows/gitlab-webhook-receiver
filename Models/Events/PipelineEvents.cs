namespace gitlab_webhook_receiver.Models.Events;

public record PipelineStarted(
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Ref,
    string Sha,
    string Status,
    DateTime StartedAt,
    string? Source,
    string Username,
    string? CommitMessage
);

public record PipelineCompleted(
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Ref,
    string Sha,
    string Status,
    DateTime CompletedAt,
    int? DurationSeconds,
    int? QueuedDurationSeconds,
    string Username,
    List<string>? Stages
);

public record PipelineFailed(
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Ref,
    string Sha,
    DateTime FailedAt,
    int? DurationSeconds,
    string Username,
    string? DetailedStatus
);

public record PipelineCanceled(
    int PipelineId,
    int ProjectId,
    string ProjectName,
    string Ref,
    DateTime CanceledAt,
    string Username
);
