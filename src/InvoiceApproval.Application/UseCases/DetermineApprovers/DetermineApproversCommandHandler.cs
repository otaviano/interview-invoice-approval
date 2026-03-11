using FluentValidation;
using InvoiceApproval.Domain.Services;
using MediatR;

namespace InvoiceApproval.Application.UseCases.DetermineApprovers;

public sealed class DetermineApproversCommandHandler(
    IValidator<DetermineApproversCommand> validator,
    IApprovalWorkflowService approvalWorkflowService)
    : IRequestHandler<DetermineApproversCommand, DetermineApproversResult>
{
    public async Task<DetermineApproversResult> Handle(
        DetermineApproversCommand command,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var approvers = approvalWorkflowService.DetermineApprovers(
            command.Amount,
            command.IsPreferredVendor);

        return new DetermineApproversResult(approvers);
    }
}
