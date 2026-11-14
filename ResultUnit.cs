using System.Data;

namespace EcoSQL.Request;

/// <summary>Represents a unit of operation that receives an item collection
/// of a specific type as its result.</summary>
/// <typeparam name="TItem">The type of the items in the collection.</typeparam>
public class ResultCollectionUnit<TItem> : ResultUnit<ICollection<TItem>>
{
    /// <summary>Gets or sets the result item collection of the operation.</summary>
    public override ICollection<TItem>? Result
    {
        get => base.Result?.ToList();
        set => base.Result = value;
    }

    /// <summary>Adds a new item to the <see cref="Result"/> collection.</summary>
    /// <param name="newItem">The new item to add.</param>
    public virtual void AddItem(TItem newItem)
    {
        base.Result ??= [];
        base.Result.Add(newItem);
        OnResultCollectionItemAdded(newItem);
    }

    /// <summary>Removes an item from the <see cref="Result"/> collection.</summary>
    /// <param name="item">The item to remove.</param>
    public virtual void RemoveItem(TItem item)
    {
        base.Result ??= [];
        base.Result.Remove(item);
        OnResultCollectionItemRemoved(item);
    }

    /// <summary>Occurs when a new item is added to the <see cref="Result"/> collection.</summary>
    public event Action<TItem>? ResultCollectionItemAdded;
    /// <summary>Occurs when an item is removed from the <see cref="Result"/> collection.</summary>
    public event Action<TItem>? ResultCollectionItemRemoved;

    /// <summary>Raises the <see cref="ResultCollectionItemAdded"/> event.</summary>
    protected void OnResultCollectionItemAdded(TItem item) =>
        ResultCollectionItemAdded?.Invoke(item);

    /// <summary>Raises the <see cref="ResultCollectionItemRemoved"/> event.</summary>
    protected void OnResultCollectionItemRemoved(TItem item) =>
        ResultCollectionItemRemoved?.Invoke(item);
}

/// <summary>Represents a unit of operation that receives a <see cref="DataTable"/>
/// as its result.</summary>
public class ResultTableUnit : ResultUnit<DataTable>
{
    /// <summary>Adds a new <see cref="DataRow"/> to the <see cref="Result"/> table.</summary>
    /// <param name="newRow">The new row to add.</param>
    public virtual void AddDataRow(DataRow newRow)
    {
        Result ??= new();
        Result.Rows.Add(newRow);
        OnResultTableRowAdded(newRow);
    }

    /// <summary>Removes a <see cref="DataRow"/> from the <see cref="Result"/> table.</summary>
    /// <param name="row">The row to remove.</param>
    public virtual void RemoveDataRow(DataRow row)
    {
        Result ??= new();
        Result.Rows.Remove(row);
        OnResultTableRowRemoved(row);
    }

    /// <summary>Occurs when a new <see cref="DataRow"/> is added to the <see cref="Result"/> table.</summary>
    public event Action<DataRow>? ResultTableRowAdded;
    /// <summary>Occurs when a <see cref="DataRow"/> is removed from the <see cref="Result"/> table.</summary>
    public event Action<DataRow>? ResultTableRowRemoved;

    /// <summary>Raises the <see cref="ResultTableRowAdded"/> event.</summary>
    protected void OnResultTableRowAdded(DataRow row) =>
        ResultTableRowAdded?.Invoke(row);

    /// <summary>Raises the <see cref="ResultTableRowRemoved"/> event.</summary>
    protected void OnResultTableRowRemoved(DataRow row) =>
        ResultTableRowRemoved?.Invoke(row);
}

/// <summary>Represents a unit of operation that receives a result of a specific type.</summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class ResultUnit<TResult> : OperationUnit
{
    /// <summary>Gets or sets the result of the operation.</summary>
    public virtual TResult? Result
    {
        get;
        set
        {
            field = value;
            OnResultChanged();
        }
    }

    /// <summary>Occurs when the <see cref="Result"/> property changes.</summary>
    public event Action<TResult?>? ResultChanged;

    /// <summary>Raises the <see cref="ResultChanged"/> event.</summary>
    protected void OnResultChanged() => ResultChanged?.Invoke(Result);
}