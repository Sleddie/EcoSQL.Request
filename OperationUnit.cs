namespace EcoSQL.Request;

/// <summary>Represents a unit that can trigger events based on message and count changes.</summary>
public class OperationUnit : IMessageUnit, ICounterUnit
{
    /// <summary>Gets or sets the current message.</summary>
    public virtual string? Message
    {
        get;
        set
        {
            field = value;
            OnMessageChanged();
        }
    }

    /// <summary>Gets or sets the progress count.</summary>
    public virtual uint Count
    {
        get;
        set
        {
            field = value;
            OnCountChanged();
        }
    }

    /// <summary>Gets or sets the total count.</summary>
    public virtual uint TotalCount
    {
        get;
        set
        {
            field = value;
            OnTotalCountChanged();
        }
    }

    /// <summary>Adjusts the progress count by a specified delta.</summary>
    /// <param name="delta">The delta by which to adjust <see cref="Count"/>.</param>
    public virtual void AdjustCount(int delta = 1)
    {
        uint absDelta = (uint)Math.Abs(delta);

        if (delta > 0 && absDelta <= uint.MaxValue - Count)
            Count += absDelta;
        else if (delta < 0 && absDelta <= Count)
            Count -= absDelta;

        OnCountAdjusted(delta);
    }

    /// <summary>Occurs when the <see cref="Message"/> property changes.</summary>
    public event Action<string?>? MessageChanged;
    /// <summary>Occurs when the <see cref="Count"/> property changes.</summary>
    public event Action<uint>? CountChanged;
    /// <summary>Occurs when the <see cref="Count"/> property is adjusted by a delta.</summary>
    public event Action<int>? CountAdjusted;
    /// <summary>Occurs when the <see cref="TotalCount"/> property changes.</summary>
    public event Action<uint>? TotalCountChanged;

    /// <summary>Raises the <see cref="MessageChanged"/> event.</summary>
    protected void OnMessageChanged() => MessageChanged?.Invoke(Message);

    /// <summary>Raises the <see cref="CountChanged"/> event.</summary>
    protected void OnCountChanged() => CountChanged?.Invoke(Count);

    /// <summary>Raises the <see cref="CountAdjusted"/> event.</summary>
    protected void OnCountAdjusted(int delta) => CountAdjusted?.Invoke(delta);

    /// <summary>Raises the <see cref="TotalCountChanged"/> event.</summary>
    protected void OnTotalCountChanged() => TotalCountChanged?.Invoke(TotalCount);

    /// <summary>Increments the <see cref="Count"/> by 1.</summary>
    public static OperationUnit operator ++(OperationUnit countUnit)
    {
        countUnit.AdjustCount(1);
        return countUnit;
    }

    /// <summary>Decrements the <see cref="Count"/> by 1.</summary>
    public static OperationUnit operator --(OperationUnit countUnit)
    {
        countUnit.AdjustCount(-1);
        return countUnit;
    }
}

/// <summary>Represents an interface for counter trigger units.</summary>
public interface ICounterUnit
{
    /// <summary>Gets or sets the progress count.</summary>
    uint Count { get; set; }
    /// <summary>Gets or sets the total count.</summary>
    uint TotalCount { get; set; }

    /// <summary>Adjusts the progress count by a specified delta.</summary>
    /// <param name="delta">The delta by which to adjust <see cref="Count"/>.</param>
    void AdjustCount(int delta = 1);

    /// <summary>Occurs when the <see cref="Count"/> property changes.</summary>
    event Action<uint>? CountChanged;
    /// <summary>Occurs when the <see cref="Count"/> property is adjusted by a delta.</summary>
    event Action<int>? CountAdjusted;
    /// <summary>Occurs when the <see cref="TotalCount"/> property changes.</summary>
    event Action<uint>? TotalCountChanged;
}

/// <summary>Represents an interface for message trigger units.</summary>
public interface IMessageUnit
{
    /// <summary>Gets or sets the current message.</summary>
    string? Message { get; set; }

    /// <summary>Occurs when the <see cref="Message"/> property changes.</summary>
    event Action<string?>? MessageChanged;
}