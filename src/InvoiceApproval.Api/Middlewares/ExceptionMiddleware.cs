using System.Net;
using FluentValidation;
using InvoiceApproval.Infra.Core;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApproval.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
    {
        try
        {
            await next(httpContext);
        }
        catch (ValidationException e)
        {
            await SetResponseError(
                Problems.ValidationError.Type,
                Problems.ValidationError.Title,
                HttpStatusCode.UnprocessableContent,
                httpContext,
                logger,
                e);
        }
        catch (Exception e)
        {
            await SetResponseError(
                Problems.Error.Type,
                Problems.Error.Title,
                HttpStatusCode.InternalServerError,
                httpContext,
                logger,
                e);
        }
    }

    private static async Task SetResponseError(
        string type,
        string? title,
        HttpStatusCode statusCode,
        HttpContext httpContext,
        ILogger<ExceptionMiddleware> logger,
        Exception e)
    {
        var isProduction = httpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsProduction();

        var problem = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = (int)statusCode,
            Extensions = new Dictionary<string, object?> { { "Errors", e.Message } }
        };

        if (!isProduction)
            problem.Detail = e.StackTrace;

        httpContext.Response.StatusCode = (int)statusCode;
        logger.LogError(e, "{Title}", title);
        await httpContext.Response.WriteAsJsonAsync(problem);
    }
}
