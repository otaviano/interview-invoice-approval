using FluentValidation;

namespace InvoiceApproval.Application.UseCases.DetermineApprovers;

public sealed class DetermineApproversCommandValidator : AbstractValidator<DetermineApproversCommand>
{
    public DetermineApproversCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Invoice amount must be greater than zero")
            .LessThanOrEqualTo(999_999_999_999.99m)
            .WithMessage("Invoice amount exceeds the maximum allowed value");
    }
}
