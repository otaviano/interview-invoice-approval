using FluentValidation;
using InvoiceApproval.Api.Extensions;
using InvoiceApproval.Api.Middlewares;
using InvoiceApproval.Api.Validators;
using InvoiceApproval.Infra.IoC;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssemblyContaining<DetermineApproversRequestValidator>();
builder.Services.AddDomainServices();
builder.Services.AddUseCases();
builder.Services.AddValidations();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseCors();
app.UseMiddleware<ExceptionMiddleware>();
app.MapEndpoints();

app.Run();

public partial class Program { }
