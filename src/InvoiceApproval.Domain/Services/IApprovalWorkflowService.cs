namespace InvoiceApproval.Domain.Services;

public interface IApprovalWorkflowService
{
    IReadOnlyList<string> DetermineApprovers(decimal amount, bool isPreferredVendor);
}
