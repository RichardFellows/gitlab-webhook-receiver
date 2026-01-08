using System.Text.Json.Serialization;
using gitlab_webhook_receiver.Models.Webhooks.Common;

namespace gitlab_webhook_receiver.Models.Webhooks;

public class JobWebhook
{
    [JsonPropertyName("object_kind")]
    public string ObjectKind { get; set; } = string.Empty;

    [JsonPropertyName("ref")]
    public string? Ref { get; set; }

    [JsonPropertyName("tag")]
    public bool Tag { get; set; }

    [JsonPropertyName("before_sha")]
    public string? BeforeSha { get; set; }

    [JsonPropertyName("sha")]
    public string? Sha { get; set; }

    [JsonPropertyName("build_id")]
    public int BuildId { get; set; }

    [JsonPropertyName("build_name")]
    public string BuildName { get; set; } = string.Empty;

    [JsonPropertyName("build_stage")]
    public string BuildStage { get; set; } = string.Empty;

    [JsonPropertyName("build_status")]
    public string BuildStatus { get; set; } = string.Empty;

    [JsonPropertyName("build_created_at")]
    public DateTime? BuildCreatedAt { get; set; }

    [JsonPropertyName("build_started_at")]
    public DateTime? BuildStartedAt { get; set; }

    [JsonPropertyName("build_finished_at")]
    public DateTime? BuildFinishedAt { get; set; }

    [JsonPropertyName("build_duration")]
    public double? BuildDuration { get; set; }

    [JsonPropertyName("build_queued_duration")]
    public double? BuildQueuedDuration { get; set; }

    [JsonPropertyName("build_allow_failure")]
    public bool BuildAllowFailure { get; set; }

    [JsonPropertyName("build_failure_reason")]
    public string? BuildFailureReason { get; set; }

    [JsonPropertyName("pipeline_id")]
    public int PipelineId { get; set; }

    [JsonPropertyName("runner")]
    public Runner? Runner { get; set; }

    [JsonPropertyName("project_id")]
    public int ProjectId { get; set; }

    [JsonPropertyName("project_name")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("commit")]
    public Commit? Commit { get; set; }

    [JsonPropertyName("repository")]
    public Repository? Repository { get; set; }

    [JsonPropertyName("environment")]
    public Common.Environment? Environment { get; set; }
}

public class Repository
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonPropertyName("git_http_url")]
    public string? GitHttpUrl { get; set; }

    [JsonPropertyName("git_ssh_url")]
    public string? GitSshUrl { get; set; }

    [JsonPropertyName("visibility_level")]
    public int VisibilityLevel { get; set; }
}
