using InvoiceApproval.Domain.Enums;

namespace InvoiceApproval.Domain.Handlers;

public sealed class ManagerApprovalHandler : ApprovalHandler
{
    public override void Handle(ApprovalContext context)
    {
        if (context.IsPreferredVendor && !context.PreferredVendorSkipApplied)
            context.PreferredVendorSkipApplied = true;
        else
            context.Approvers.Add(ApproverLevel.Manager.ToString());

        base.Handle(context);
    }
}
