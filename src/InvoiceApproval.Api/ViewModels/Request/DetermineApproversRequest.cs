namespace InvoiceApproval.Api.ViewModels.Request;

public sealed class DetermineApproversRequest
{
    public decimal Amount { get; init; }
    public bool IsPreferredVendor { get; init; }
}
