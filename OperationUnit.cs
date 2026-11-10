namespace EcoSQL.Request;

public class OperationUnit : IMessageUnit, ICounterUnit
{
    public virtual string? Message
    {
        get;
        set
        {
            field = value;
            OnMessageChanged();
        }
    }

    public virtual uint Count
    {
        get;
        set
        {
            field = value;
            OnCountChanged();
        }
    }

    public virtual uint TotalCount
    {
        get;
        set
        {
            field = value;
            OnTotalCountChanged();
        }
    }

    public virtual void AdjustCount(int delta = 1)
    {
        uint absDelta = (uint)Math.Abs(delta);

        if (delta > 0 && absDelta <= uint.MaxValue - Count)
            Count += absDelta;
        else if (delta < 0 && absDelta <= Count)
            Count -= absDelta;

        OnCountAdjusted(delta);
    }

    public event Action<string?>? MessageChanged;
    public event Action<uint>? CountChanged;
    public event Action<int>? CountAdjusted;
    public event Action<uint>? TotalCountChanged;

    protected void OnMessageChanged()
    {
        MessageChanged?.Invoke(Message);
    }

    protected void OnCountChanged()
    {
        CountChanged?.Invoke(Count);
    }

    protected void OnCountAdjusted(int delta)
    {
        CountAdjusted?.Invoke(delta);
    }

    protected void OnTotalCountChanged()
    {
        TotalCountChanged?.Invoke(TotalCount);
    }

    public static OperationUnit operator ++(OperationUnit countUnit)
    {
        countUnit.AdjustCount(1);
        return countUnit;
    }

    public static OperationUnit operator --(OperationUnit countUnit)
    {
        countUnit.AdjustCount(-1);
        return countUnit;
    }
}

public interface ICounterUnit
{
    uint Count { get; set; }
    uint TotalCount { get; set; }

    void AdjustCount(int delta = 1);

    event Action<uint>? CountChanged;
    event Action<int>? CountAdjusted;
    event Action<uint>? TotalCountChanged;
}

public interface IMessageUnit
{
    string? Message { get; set; }

    event Action<string?>? MessageChanged;
}