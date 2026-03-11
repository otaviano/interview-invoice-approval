using InvoiceApproval.Domain.Handlers;

namespace InvoiceApproval.Domain.Services;

public sealed class ApprovalWorkflowService(ApprovalHandler chain) : IApprovalWorkflowService
{
    public IReadOnlyList<string> DetermineApprovers(decimal amount, bool isPreferredVendor)
    {
        var context = new ApprovalContext(amount, isPreferredVendor);
        chain.Handle(context);
        return context.Approvers.AsReadOnly();
    }
}
