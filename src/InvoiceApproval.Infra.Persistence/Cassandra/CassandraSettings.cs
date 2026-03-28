namespace InvoiceApproval.Infra.Persistence.Cassandra;

public sealed class CassandraSettings
{
    public const string SectionName = "Cassandra";

    public string[] ContactPoints { get; init; } = ["localhost"];
    public int Port { get; init; } = 9042;
    public string Keyspace { get; init; } = "invoice_approval";
}
