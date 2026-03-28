using Cassandra;
using InvoiceApproval.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace InvoiceApproval.Infra.Persistence.Cassandra;

public sealed class CassandraApprovalRecordRepository(
    ISession session,
    ILogger<CassandraApprovalRecordRepository> logger)
    : IApprovalRecordRepository
{
    private const string InsertCql =
        "INSERT INTO approval_records (id, amount, is_preferred_vendor, approvers, created_at) " +
        "VALUES (?, ?, ?, ?, ?)";

    private PreparedStatement? _preparedInsert;

    public async Task SaveAsync(ApprovalRecord record, CancellationToken cancellationToken = default)
    {
        _preparedInsert ??= await session.PrepareAsync(InsertCql);

        var bound = _preparedInsert.Bind(
            record.Id,
            record.Amount,
            record.IsPreferredVendor,
            record.Approvers.ToList(),
            record.CreatedAt.UtcDateTime);

        await session.ExecuteAsync(bound);

        logger.LogInformation("Saved approval record {Id} for amount {Amount}", record.Id, record.Amount);
    }
}
