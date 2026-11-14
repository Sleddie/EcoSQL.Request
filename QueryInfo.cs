using System.ComponentModel.DataAnnotations;

namespace EcoSQL.Request;

public class QueryInfo : IValidatableObject
{
    public required string Text { get; set; }
    public int CommandTimeout { get; set; } = 120;
    public CancellationToken? CancellationToken { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            yield return new("Empty query text.", [nameof(Text)]);
        }
    }

    public static implicit operator QueryInfo(string queryText) =>
        new() { Text = queryText };
}