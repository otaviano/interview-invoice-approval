# Invoice Approval – Project Patterns

## Architecture

Clean Architecture with CQRS (MediatR) and Cassandra persistence.

```
src/
├── InvoiceApproval.Api              → Endpoints, ViewModels, Filters, Middlewares
├── InvoiceApproval.Application      → UseCases (Commands, Handlers, Validators, Results), Repository interfaces
├── InvoiceApproval.Domain           → Entities, Enums, Service interfaces + implementations
├── InvoiceApproval.Infra.Core       → Shared cross-cutting concerns (Problems)
├── InvoiceApproval.Infra.IoC        → DI registration extensions
└── InvoiceApproval.Infra.Persistence → Cassandra session factory + repository implementations
tests/
└── InvoiceApproval.Tests.Unit       → Mirrors src/ structure
frontend/                            → Vue 3 + Vuetify 3 + Vite + TypeScript
infra/cassandra/                     → init.cql schema script
```

**Dependency rules:** Domain → nothing | Application → Domain | Infra.Persistence → Application | Infra.IoC → Application, Domain, Infra.Persistence | Api → Infra.Core, Infra.IoC

**Repository interfaces** live in `Application/Repositories/` — never in Domain or Infra layers.

## Tech Stack

- .NET 10.0, C# latest, `nullable enable`, `ImplicitUsings enable`, file-scoped namespaces
- MediatR 12.x, FluentValidation 11.x, Scalar.AspNetCore
- CassandraCSharpDriver 3.21.x (DataStax) — keyspace `invoice_approval`, table `approval_records`
- xUnit 2.9.x, NSubstitute 5.x, FluentAssertions 6.x
- Frontend: Vue 3.5, Vuetify 3.7, Vite 6, Axios, TypeScript 5.7
- Docker: `docker-compose up --build` runs Cassandra 4.1 + the API on port 8080

## Adding a New Feature (Use Case)

### 1. Domain Layer

Create interface + sealed implementation in `Domain/Services/`:

```csharp
// IMyService.cs
namespace InvoiceApproval.Domain.Services;
public interface IMyService
{
    IReadOnlyList<string> DoSomething(decimal value);
}

// MyService.cs
namespace InvoiceApproval.Domain.Services;
public sealed class MyService : IMyService
{
    public IReadOnlyList<string> DoSomething(decimal value) { /* ... */ }
}
```

### 2. Application Layer

Create a folder `Application/UseCases/{FeatureName}/` with 4 files:

**Command** – positional `record` implementing `IRequest<TResult>`:

```csharp
namespace InvoiceApproval.Application.UseCases.FeatureName;
public record FeatureNameCommand(decimal Amount, bool Flag)
    : IRequest<FeatureNameResult>;
```

**Result** – positional `record`:

```csharp
namespace InvoiceApproval.Application.UseCases.FeatureName;
public record FeatureNameResult(IReadOnlyList<string> Items);
```

**Validator** – `sealed class` extending `AbstractValidator<TCommand>`:

```csharp
namespace InvoiceApproval.Application.UseCases.FeatureName;
public sealed class FeatureNameCommandValidator : AbstractValidator<FeatureNameCommand>
{
    public FeatureNameCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");
    }
}
```

**Handler** – `sealed class` with primary constructor, inject `IValidator<T>` + domain service + repository:

```csharp
namespace InvoiceApproval.Application.UseCases.FeatureName;
public sealed class FeatureNameCommandHandler(
    IValidator<FeatureNameCommand> validator,
    IMyService myService,
    IApprovalRecordRepository approvalRecordRepository)
    : IRequestHandler<FeatureNameCommand, FeatureNameResult>
{
    public async Task<FeatureNameResult> Handle(
        FeatureNameCommand command,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var result = myService.DoSomething(command.Amount);
        // persist if needed
        return new FeatureNameResult(result);
    }
}
```

### 3. API Layer

**Request/Response ViewModels** in `Api/ViewModels/Request/` and `Api/ViewModels/Response/`:

```csharp
// sealed class, init properties
public sealed class FeatureNameRequest
{
    public decimal Amount { get; init; }
    public bool Flag { get; init; }
}
```

**Request Validator** in `Api/Validators/`:

```csharp
public sealed class FeatureNameRequestValidator : AbstractValidator<FeatureNameRequest>
{
    public FeatureNameRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
```

**Endpoints** in `Api/Endpoints/` – static class, extension method on `WebApplication`:

```csharp
namespace InvoiceApproval.Api.Endpoints;
public static class ResourceEndpoints
{
    public static void MapResourceEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/resource")
            .WithTags("Resource");

        endpoints.MapPost("/action", ActionMethod)
            .AddEndpointFilter<ValidationFilter<FeatureNameRequest>>()
            .WithName("ActionName")
            .WithSummary("Description of endpoint")
            .Produces<FeatureNameResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> ActionMethod(
        [FromBody] FeatureNameRequest request,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new FeatureNameCommand(request.Amount, request.Flag);
        var result = await mediator.Send(command, cancellationToken);
        var response = new FeatureNameResponse { Items = result.Items };
        return Results.Ok(response);
    }
}
```

Register in `Api/Extensions/EndpointsExtensions.cs` via `app.MapResourceEndpoints()`.

### 4. DI Registration

Add extension method in `Infra.IoC/` following existing pattern:

```csharp
public static IServiceCollection AddDomainServices(this IServiceCollection services)
{
    services.AddScoped<IMyService, MyService>();
    return services;
}
```

Register in `Program.cs` via `builder.Services.AddXxx()`.

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Namespace | `InvoiceApproval.{Layer}.{Folder}` | `InvoiceApproval.Application.UseCases.DetermineApprovers` |
| Interface | `I` prefix | `IApprovalWorkflowService` |
| Service impl | `sealed class` | `ApprovalWorkflowService` |
| Repository interface | `I{Name}Repository` in `Application/Repositories/` | `IApprovalRecordRepository` |
| Repository impl | `Cassandra{Name}Repository` in `Infra.Persistence/Cassandra/` | `CassandraApprovalRecordRepository` |
| Command | positional `record` | `DetermineApproversCommand(decimal Amount, bool IsPreferredVendor)` |
| Result | positional `record` | `DetermineApproversResult(IReadOnlyList<string> Approvers)` |
| Handler | `sealed class`, primary ctor | `DetermineApproversCommandHandler` |
| Validator | `sealed class` | `DetermineApproversCommandValidator` |
| Endpoint class | `static class`, plural | `InvoicesEndpoints` |
| Endpoint method | `Map{Resource}Endpoints` | `MapInvoicesEndpoints` |
| DI extension | `Add{Category}` | `AddDomainServices`, `AddUseCases`, `AddPersistence` |
| Request VM | `sealed class`, `init` props | `DetermineApproversRequest` |
| Response VM | `sealed class`, `init` props | `DetermineApproversResponse` |

## C# Style Rules

- File-scoped namespaces (`namespace X;`)
- Primary constructors for handlers, filters, middleware
- `sealed` on all non-static, non-abstract classes
- Digit separators for large numbers (`1_000m`, `999_999_999_999.99m`)
- Pattern matching with `switch` expressions
- Collection expressions (`[item1, item2]`) for static arrays
- Named arguments when boolean intent is unclear (`isPreferredVendor: true`)
- **Always fail fast** — use early returns / guard clauses instead of nested `if` blocks

## Persistence Conventions

- `ISession` registered as **Singleton**; repositories registered as **Scoped**
- DDL runs idempotently on startup via `CassandraSessionFactory.EnsureSchemaAsync` (`CREATE ... IF NOT EXISTS`)
- Cassandra column naming: `snake_case` (e.g. `is_preferred_vendor`, `created_at`)
- Use `PreparedStatement` for all DML; prepare lazily on first call
- `list<text>` columns require `List<string>` (not `IReadOnlyList`) when binding
- `DateTimeOffset` → pass `.UtcDateTime` to the driver for `timestamp` columns
- Cassandra settings bound from `appsettings.json` section `"Cassandra"` (overridable via `Cassandra__ContactPoints__0` env var in Docker)

## Testing Conventions

Tests mirror `src/` structure under `tests/InvoiceApproval.Tests.Unit/`.

### Naming

- Class: `{ClassUnderTest}Tests` (e.g. `ApprovalWorkflowServiceTests`)
- Method: `MethodName_Scenario_ExpectedBehavior` (e.g. `DetermineApprovers_UnderOneThousand_ShouldReturnManager`)

### Structure

- SUT variable: `_sut`
- Mocks: `Substitute.For<T>()` as `readonly` fields
- SUT instantiation: constructor or field initializer
- `[Theory]` + `[InlineData]` for parameterized tests
- `[Fact]` for single-scenario tests
- Mock `IApprovalRecordRepository` in handler tests — default NSubstitute behavior returns completed `Task`

### Assertions (FluentAssertions)

```csharp
result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
result.Should().BeEmpty();
result.Should().NotContain("Manager");
result.Should().NotBeNull();
await act.Should().ThrowAsync<ValidationException>();
```

### Handler Test Pattern

```csharp
public class FeatureNameCommandHandlerTests
{
    private readonly IValidator<FeatureNameCommand> _validator = Substitute.For<IValidator<FeatureNameCommand>>();
    private readonly IMyService _myService = Substitute.For<IMyService>();
    private readonly IApprovalRecordRepository _approvalRecordRepository = Substitute.For<IApprovalRecordRepository>();
    private readonly FeatureNameCommandHandler _sut;

    public FeatureNameCommandHandlerTests()
    {
        _sut = new FeatureNameCommandHandler(_validator, _myService, _approvalRecordRepository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnResult()
    {
        var command = new FeatureNameCommand(5000m, false);
        _myService.DoSomething(command.Amount).Returns(new List<string> { "Item" }.AsReadOnly());

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(new[] { "Item" });
    }
}
```

## Error Handling

- `ExceptionMiddleware` catches `ValidationException` → 422 and generic `Exception` → 500
- Responses use `ProblemDetails` with types defined in `Infra.Core/Problems.cs`
- Stack traces only included in non-production environments
- `ValidationFilter<T>` on endpoints catches request-level validation before MediatR

## API Conventions

- Route: `api/{resource}/{action}` (kebab-case)
- HTTP verbs: `POST` for commands
- Endpoint groups with `.WithTags("ResourceName")`
- `.WithName()` and `.WithSummary()` for OpenAPI
- `.Produces<T>()` and `.ProducesValidationProblem()` for response documentation
- Scalar API reference at `/scalar/v1`
