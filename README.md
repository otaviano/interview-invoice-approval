# Invoice Approval Workflow

A .NET 10.0 + Vue 3 application that determines the required approvers for an invoice based on its amount and vendor status.

## Business Rules

| Invoice Amount | Required Approvers |
|---|---|
| < 1,000 | Manager |
| 1,000 – 9,999.99 | Manager, Director |
| >= 10,000 | Manager, Director, VP |

**Preferred vendors** skip the lowest-level approver in the chain.

## Tech Stack

### Backend

- .NET 10.0 / C# latest
- MediatR (CQRS)
- FluentValidation
- Scalar (OpenAPI docs)
- xUnit + NSubstitute + FluentAssertions

### Frontend

- Vue 3 + TypeScript
- Vuetify 3
- Vite
- Axios

## Architecture

Clean Architecture with CQRS via MediatR and Chain of Responsibility for approval logic.

```
src/
├── InvoiceApproval.Api           → Minimal API endpoints, middlewares, view models
├── InvoiceApproval.Application   → Use cases (commands, handlers, validators, results)
├── InvoiceApproval.Domain        → Business rules, enums, service interfaces, handlers
├── InvoiceApproval.Infra.Core    → Shared cross-cutting concerns (ProblemDetails)
└── InvoiceApproval.Infra.IoC     → Dependency injection setup
tests/
└── InvoiceApproval.Tests.Unit    → Unit tests (mirrors src/ structure)
frontend/                         → Vue 3 SPA
```

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)

## How to Run

### Backend

```bash
dotnet run --project src/InvoiceApproval.Api
```

The API will be available at **http://localhost:5000**.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The app will be available at **http://localhost:8080**.

## API

### OpenAPI Documentation

- Scalar UI: http://localhost:5000/scalar/v1
- OpenAPI JSON: http://localhost:5000/openapi/v1.json

### Determine Approvers

```
POST /api/invoices/determine-approvers
Content-Type: application/json

{
  "amount": 15000,
  "isPreferredVendor": false
}
```

**Response:**

```json
{
  "approvers": ["Manager", "Director", "VP"]
}
```

## Running Tests

```bash
dotnet test
```
