namespace InvoiceApproval.Infra.Core;

public static class Problems
{
    public static readonly ProblemInfo ValidationError = new("validation-error", "Validation Error");
    public static readonly ProblemInfo Error = new("internal-error", "Internal Server Error");

    public record ProblemInfo(string Type, string Title);
}
