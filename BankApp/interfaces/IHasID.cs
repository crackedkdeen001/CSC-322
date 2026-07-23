namespace BankApp.interfaces;

/// <summary>
/// Implemented by anything the repository stores, so it can be looked up and matched by a unique id.
/// </summary>
public interface IHasID
{
    /// <summary>
    /// The unique identifier of the item. Assigned by the repository when the item is first added.
    /// </summary>
    public int Id { get; set; }
}