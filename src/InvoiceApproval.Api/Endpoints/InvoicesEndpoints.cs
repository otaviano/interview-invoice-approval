using InvoiceApproval.Api.Filters;
using InvoiceApproval.Api.ViewModels.Request;
using InvoiceApproval.Api.ViewModels.Response;
using InvoiceApproval.Application.UseCases.DetermineApprovers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApproval.Api.Endpoints;

public static class InvoicesEndpoints
{
    public static void MapInvoicesEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/invoices")
            .WithTags("Invoices");

        endpoints.MapPost("/determine-approvers", DetermineApprovers)
            .AddEndpointFilter<ValidationFilter<DetermineApproversRequest>>()
            .WithName("DetermineApprovers")
            .WithSummary("Determine required approvers for an invoice based on amount and vendor status")
            .Produces<DetermineApproversResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> DetermineApprovers(
        [FromBody] DetermineApproversRequest request,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new DetermineApproversCommand(request.Amount, request.IsPreferredVendor);
        var result = await mediator.Send(command, cancellationToken);

        var response = new DetermineApproversResponse { Approvers = result.Approvers };
        return Results.Ok(response);
    }
}
