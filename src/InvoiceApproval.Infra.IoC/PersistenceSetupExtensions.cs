using Cassandra;
using InvoiceApproval.Application.Repositories;
using InvoiceApproval.Infra.Persistence.Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvoiceApproval.Infra.IoC;

public static class PersistenceSetupExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection(CassandraSettings.SectionName)
            .Get<CassandraSettings>() ?? new CassandraSettings();

        services.AddSingleton(settings);

        services.AddSingleton<ISession>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("CassandraSessionFactory");
            return CassandraSessionFactory.CreateSessionAsync(settings, logger)
                .GetAwaiter().GetResult();
        });

        services.AddScoped<IApprovalRecordRepository, CassandraApprovalRecordRepository>();

        return services;
    }
}
