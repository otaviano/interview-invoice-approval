namespace InvoiceApproval.Application.UseCases.DetermineApprovers;

public record DetermineApproversResult(IReadOnlyList<string> Approvers);
