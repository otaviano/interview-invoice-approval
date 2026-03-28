namespace InvoiceApproval.Application.Repositories;

public interface IApprovalRecordRepository
{
    Task SaveAsync(ApprovalRecord record, CancellationToken cancellationToken = default);
}
