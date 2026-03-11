using FluentValidation;
using InvoiceApproval.Api.ViewModels.Request;

namespace InvoiceApproval.Api.Validators;

public sealed class DetermineApproversRequestValidator : AbstractValidator<DetermineApproversRequest>
{
    public DetermineApproversRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Invoice amount must be greater than zero")
            .LessThanOrEqualTo(999_999_999_999.99m)
            .WithMessage("Invoice amount exceeds the maximum allowed value");
    }
}
