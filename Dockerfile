# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY InvoiceApproval.slnx ./
COPY src/InvoiceApproval.Api/InvoiceApproval.Api.csproj                               src/InvoiceApproval.Api/
COPY src/InvoiceApproval.Application/InvoiceApproval.Application.csproj               src/InvoiceApproval.Application/
COPY src/InvoiceApproval.Domain/InvoiceApproval.Domain.csproj                         src/InvoiceApproval.Domain/
COPY src/InvoiceApproval.Infra.Core/InvoiceApproval.Infra.Core.csproj                 src/InvoiceApproval.Infra.Core/
COPY src/InvoiceApproval.Infra.IoC/InvoiceApproval.Infra.IoC.csproj                   src/InvoiceApproval.Infra.IoC/
COPY src/InvoiceApproval.Infra.Persistence/InvoiceApproval.Infra.Persistence.csproj   src/InvoiceApproval.Infra.Persistence/

RUN dotnet restore InvoiceApproval.slnx

COPY src/ src/
RUN dotnet publish src/InvoiceApproval.Api/InvoiceApproval.Api.csproj \
    -c Release -o /app/publish --no-restore

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser && chown appuser /app
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "InvoiceApproval.Api.dll"]
