using System.Text.Json.Serialization;
using gitlab_webhook_receiver.Models.Webhooks.Common;

namespace gitlab_webhook_receiver.Models.Webhooks;

public class PipelineWebhook
{
    [JsonPropertyName("object_kind")]
    public string ObjectKind { get; set; } = string.Empty;

    [JsonPropertyName("object_attributes")]
    public PipelineAttributes ObjectAttributes { get; set; } = new();

    [JsonPropertyName("merge_request")]
    public MergeRequest? MergeRequest { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("project")]
    public Project Project { get; set; } = new();

    [JsonPropertyName("commit")]
    public Commit? Commit { get; set; }

    [JsonPropertyName("builds")]
    public List<Build>? Builds { get; set; }
}

public class PipelineAttributes
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("iid")]
    public int? Iid { get; set; }

    [JsonPropertyName("ref")]
    public string Ref { get; set; } = string.Empty;

    [JsonPropertyName("tag")]
    public bool Tag { get; set; }

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("before_sha")]
    public string? BeforeSha { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("detailed_status")]
    public string? DetailedStatus { get; set; }

    [JsonPropertyName("stages")]
    public List<string>? Stages { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("finished_at")]
    public DateTime? FinishedAt { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("queued_duration")]
    public int? QueuedDuration { get; set; }

    [JsonPropertyName("variables")]
    public List<Variable>? Variables { get; set; }
}

public class Variable
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class MergeRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("iid")]
    public int Iid { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("source_branch")]
    public string SourceBranch { get; set; } = string.Empty;

    [JsonPropertyName("source_project_id")]
    public int SourceProjectId { get; set; }

    [JsonPropertyName("target_branch")]
    public string TargetBranch { get; set; } = string.Empty;

    [JsonPropertyName("target_project_id")]
    public int TargetProjectId { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
