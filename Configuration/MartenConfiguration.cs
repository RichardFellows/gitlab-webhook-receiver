using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Weasel.Core;
using Wolverine.Marten;

namespace gitlab_webhook_receiver.Configuration;

public static class MartenConfiguration
{
    public static IServiceCollection AddMartenEventStore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("PostgreSQL connection string is required");

        services.AddMarten(connectionString)
            .IntegrateWithWolverine()  // Critical: enables Wolverine integration
            .UseLightweightSessions();  // Optimize for web scenarios

        return services;
    }
}
