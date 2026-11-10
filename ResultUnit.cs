using System.Data;

namespace EcoSQL.Request;

public class ResultCollectionUnit<TItem> : ResultUnit<ICollection<TItem>>
{
    public override ICollection<TItem>? Result
    {
        get => base.Result?.ToList();
        set => base.Result = value;
    }

    public virtual void AddItem(TItem newItem)
    {
        base.Result ??= [];
        base.Result.Add(newItem);
        OnResultCollectionItemAdded(newItem);
    }

    public virtual void RemoveItem(TItem item)
    {
        base.Result ??= [];
        base.Result.Remove(item);
        OnResultCollectionItemRemoved(item);
    }

    public event Action<TItem>? ResultCollectionItemAdded;
    public event Action<TItem>? ResultCollectionItemRemoved;

    protected void OnResultCollectionItemAdded(TItem item) =>
        ResultCollectionItemAdded?.Invoke(item);

    protected void OnResultCollectionItemRemoved(TItem item) =>
        ResultCollectionItemRemoved?.Invoke(item);
}

public class ResultTableUnit : ResultUnit<DataTable>
{
    public virtual void AddDataRow(DataRow newRow)
    {
        Result ??= new();
        Result.Rows.Add(newRow);
        OnResultTableRowAdded(newRow);
    }

    public virtual void RemoveDataRow(DataRow row)
    {
        Result ??= new();
        Result.Rows.Remove(row);
        OnResultTableRowRemoved(row);
    }

    public event Action<DataRow>? ResultTableRowAdded;
    public event Action<DataRow>? ResultTableRowRemoved;

    protected void OnResultTableRowAdded(DataRow row) =>
        ResultTableRowAdded?.Invoke(row);

    protected void OnResultTableRowRemoved(DataRow row) =>
        ResultTableRowRemoved?.Invoke(row);
}

public class ResultUnit<TResult> : OperationUnit
{
    public virtual TResult? Result
    {
        get;
        set
        {
            field = value;
            OnResultChanged();
        }
    }

    public event Action<TResult?>? ResultChanged;

    protected void OnResultChanged()
    {
        ResultChanged?.Invoke(Result);
    }
}