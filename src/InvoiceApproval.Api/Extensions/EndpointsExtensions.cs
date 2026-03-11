using InvoiceApproval.Api.Endpoints;

namespace InvoiceApproval.Api.Extensions;

public static class EndpointsExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapInvoicesEndpoints();
        return app;
    }
}
