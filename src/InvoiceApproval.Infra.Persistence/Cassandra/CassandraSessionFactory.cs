using Cassandra;
using Microsoft.Extensions.Logging;

namespace InvoiceApproval.Infra.Persistence.Cassandra;

public static class CassandraSessionFactory
{
    public static async Task<ISession> CreateSessionAsync(CassandraSettings settings, ILogger logger)
    {
        var cluster = Cluster.Builder()
            .AddContactPoints(settings.ContactPoints)
            .WithPort(settings.Port)
            .Build();

        var bootstrapSession = await cluster.ConnectAsync();
        await EnsureSchemaAsync(bootstrapSession, settings.Keyspace, logger);
        return await cluster.ConnectAsync(settings.Keyspace);
    }

    private static async Task EnsureSchemaAsync(ISession session, string keyspace, ILogger logger)
    {
        logger.LogInformation("Ensuring Cassandra schema for keyspace {Keyspace}", keyspace);

        await session.ExecuteAsync(new SimpleStatement($$"""
            CREATE KEYSPACE IF NOT EXISTS {{keyspace}}
            WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1}
            """));

        await session.ExecuteAsync(new SimpleStatement($$"""
            CREATE TABLE IF NOT EXISTS {{keyspace}}.approval_records (
                id                  uuid,
                amount              decimal,
                is_preferred_vendor boolean,
                approvers           list<text>,
                created_at          timestamp,
                PRIMARY KEY (id)
            )
            """));

        logger.LogInformation("Cassandra schema is ready");
    }
}
