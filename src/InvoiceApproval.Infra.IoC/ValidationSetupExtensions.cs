using FluentValidation;
using InvoiceApproval.Application.UseCases.DetermineApprovers;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApproval.Infra.IoC;

public static class ValidationSetupExtensions
{
    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<DetermineApproversCommandValidator>();
        return services;
    }
}
