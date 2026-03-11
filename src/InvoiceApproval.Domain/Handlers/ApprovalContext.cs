namespace InvoiceApproval.Domain.Handlers;

public sealed class ApprovalContext(decimal amount, bool isPreferredVendor)
{
    public decimal Amount { get; } = amount;
    public bool IsPreferredVendor { get; } = isPreferredVendor;
    public bool PreferredVendorSkipApplied { get; set; }
    public List<string> Approvers { get; } = [];
}
