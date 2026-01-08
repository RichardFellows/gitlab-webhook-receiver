using gitlab_webhook_receiver.Configuration;
using gitlab_webhook_receiver.Endpoints;
using gitlab_webhook_receiver.Services;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Add Marten event store with Wolverine integration
builder.Services.AddMartenEventStore(builder.Configuration);

// Add Wolverine message bus
builder.Host.UseWolverine(opts =>
{
    opts.ConfigureWolverine();
});

// Register application services
builder.Services.AddSingleton<WebhookAuthenticationService>();
builder.Services.AddSingleton<EventMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapWebhookEndpoints();
app.MapQueryEndpoints();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
