using System.ComponentModel.DataAnnotations;

namespace EcoSQL.Request;

/// <summary>Represents a database query information.</summary>
public class QueryInfo : IValidatableObject
{
    /// <summary>Gets or sets the text of the query.</summary>
    public required string Text { get; set; }
    /// <summary>Gets or sets the query timeout.</summary>
    public int CommandTimeout { get; set; } = 120;
    /// <summary>Gets or sets the cancellation token.</summary>
    public CancellationToken? CancellationToken { get; set; }

    /// <summary>Validates the query information properties.</summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>An enumerable of validation results.</returns>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            yield return new("Empty query text.", [nameof(Text)]);
        }

        if (CommandTimeout < 0)
        {
            yield return new("Command timeout is less than zero.", [nameof(CommandTimeout)]);
        }
    }

    /// <summary>Implicitly converts a string to a <see cref="QueryInfo"/> instance.</summary>
    /// <param name="queryText">The query text.</param>
    /// <returns>The <see cref="QueryInfo"/> instance.</returns>
    public static implicit operator QueryInfo(string queryText) =>
        new() { Text = queryText };
}