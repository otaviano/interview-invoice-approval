using InvoiceApproval.Application.UseCases.DetermineApprovers;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApproval.Infra.IoC;

public static class UseCaseSetupExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<DetermineApproversCommandHandler>());

        return services;
    }
}
