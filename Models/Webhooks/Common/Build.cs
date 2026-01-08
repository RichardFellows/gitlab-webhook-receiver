using System.Text.Json.Serialization;

namespace gitlab_webhook_receiver.Models.Webhooks.Common;

public class Build
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("stage")]
    public string Stage { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("started_at")]
    public DateTime? StartedAt { get; set; }

    [JsonPropertyName("finished_at")]
    public DateTime? FinishedAt { get; set; }

    [JsonPropertyName("duration")]
    public double? Duration { get; set; }

    [JsonPropertyName("queued_duration")]
    public double? QueuedDuration { get; set; }

    [JsonPropertyName("when")]
    public string? When { get; set; }

    [JsonPropertyName("manual")]
    public bool Manual { get; set; }

    [JsonPropertyName("allow_failure")]
    public bool AllowFailure { get; set; }

    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("runner")]
    public Runner? Runner { get; set; }

    [JsonPropertyName("artifacts_file")]
    public ArtifactsFile? ArtifactsFile { get; set; }

    [JsonPropertyName("environment")]
    public Environment? Environment { get; set; }
}

public class Runner
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("runner_type")]
    public string? RunnerType { get; set; }

    [JsonPropertyName("is_shared")]
    public bool IsShared { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
}

public class ArtifactsFile
{
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }
}

public class Environment
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("deployment_tier")]
    public string? DeploymentTier { get; set; }
}
