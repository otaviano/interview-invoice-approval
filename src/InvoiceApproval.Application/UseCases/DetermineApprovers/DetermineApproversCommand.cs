using MediatR;

namespace InvoiceApproval.Application.UseCases.DetermineApprovers;

public record DetermineApproversCommand(decimal Amount, bool IsPreferredVendor)
    : IRequest<DetermineApproversResult>;
