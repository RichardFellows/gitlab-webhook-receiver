using System.Text.Json.Serialization;

namespace gitlab_webhook_receiver.Models.Webhooks.Common;

public class Project
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("web_url")]
    public string WebUrl { get; set; } = string.Empty;

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("git_ssh_url")]
    public string? GitSshUrl { get; set; }

    [JsonPropertyName("git_http_url")]
    public string? GitHttpUrl { get; set; }

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;

    [JsonPropertyName("visibility_level")]
    public int VisibilityLevel { get; set; }

    [JsonPropertyName("path_with_namespace")]
    public string PathWithNamespace { get; set; } = string.Empty;

    [JsonPropertyName("default_branch")]
    public string? DefaultBranch { get; set; }
}
