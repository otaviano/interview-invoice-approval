using FluentValidation;
using InvoiceApproval.Application.Repositories;
using InvoiceApproval.Domain.Services;
using MediatR;

namespace InvoiceApproval.Application.UseCases.DetermineApprovers;

public sealed class DetermineApproversCommandHandler(
    IValidator<DetermineApproversCommand> validator,
    IApprovalWorkflowService approvalWorkflowService,
    IApprovalRecordRepository approvalRecordRepository)
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

        var record = new ApprovalRecord(
            Id: Guid.NewGuid(),
            Amount: command.Amount,
            IsPreferredVendor: command.IsPreferredVendor,
            Approvers: approvers,
            CreatedAt: DateTimeOffset.UtcNow);

        await approvalRecordRepository.SaveAsync(record, cancellationToken);

        return new DetermineApproversResult(approvers);
    }
}
