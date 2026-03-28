namespace InvoiceApproval.Application.Repositories;

public sealed record ApprovalRecord(
    Guid Id,
    decimal Amount,
    bool IsPreferredVendor,
    IReadOnlyList<string> Approvers,
    DateTimeOffset CreatedAt);
