using InvoiceApproval.Domain.Enums;

namespace InvoiceApproval.Domain.Handlers;

public sealed class VpApprovalHandler : ApprovalHandler
{
    private const decimal Threshold = 10_000m;

    public override void Handle(ApprovalContext context)
    {
        if (context.Amount < Threshold)
        {
            base.Handle(context);
            return;
        }

        if (context.IsPreferredVendor && !context.PreferredVendorSkipApplied)
            context.PreferredVendorSkipApplied = true;
        else
            context.Approvers.Add(ApproverLevel.VP.ToString());

        base.Handle(context);
    }
}
