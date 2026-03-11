namespace InvoiceApproval.Api.ViewModels.Response;

public sealed class DetermineApproversResponse
{
    public required IReadOnlyList<string> Approvers { get; init; }
}
