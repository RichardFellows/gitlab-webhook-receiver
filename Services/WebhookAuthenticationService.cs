namespace gitlab_webhook_receiver.Services;

public class WebhookAuthenticationService
{
    private readonly string _webhookSecret;
    private readonly ILogger<WebhookAuthenticationService> _logger;

    public WebhookAuthenticationService(
        IConfiguration configuration,
        ILogger<WebhookAuthenticationService> logger)
    {
        _webhookSecret = configuration["GitLab:WebhookSecret"]
            ?? throw new InvalidOperationException("GitLab:WebhookSecret configuration is required");
        _logger = logger;
    }

    public bool ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Webhook request received without token");
            return false;
        }

        var isValid = token == _webhookSecret;

        if (!isValid)
        {
            _logger.LogWarning("Webhook request received with invalid token");
        }

        return isValid;
    }
}
