using InvoiceApproval.Domain.Handlers;
using InvoiceApproval.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApproval.Infra.IoC;

public static class DomainSetupExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ManagerApprovalHandler>();
        services.AddScoped<DirectorApprovalHandler>();
        services.AddScoped<VpApprovalHandler>();

        services.AddScoped<ApprovalHandler>(sp =>
        {
            var manager = sp.GetRequiredService<ManagerApprovalHandler>();
            var director = sp.GetRequiredService<DirectorApprovalHandler>();
            var vp = sp.GetRequiredService<VpApprovalHandler>();

            manager.SetNext(director).SetNext(vp);
            return manager;
        });

        services.AddScoped<IApprovalWorkflowService, ApprovalWorkflowService>();
        return services;
    }
}
