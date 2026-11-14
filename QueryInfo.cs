namespace EcoSQL.Request;

public class QueryInfo
{
    public required string Text { get; set; }
    public int CommandTimeout { get; set; } = 120;
    public CancellationToken? CancellationToken { get; set; }

    public static implicit operator QueryInfo(string queryText) =>
        new() { Text = queryText };
}