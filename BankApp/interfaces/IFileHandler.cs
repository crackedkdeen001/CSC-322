namespace BankApp.interfaces;

/// <summary>
/// Reads and writes a whole list of items to some backing store (a JSON file, in the real implementation).
/// </summary>
/// <typeparam name="T">The type of item stored</typeparam>
public interface IFileHandler<T>
{
    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every stored item, or an empty list if nothing has been stored yet
    /// </summary>
    /// <returns>Every stored item</returns>
    List<T> LoadItems();

    /// <summary>
    /// requires: none<br/>
    /// modifies: the backing store<br/>
    /// effects: replaces the store's contents with exactly the given items
    /// </summary>
    /// <param name="items">The items to save</param>
    void SaveItems(List<T> items);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the backing store<br/>
    /// effects: removes every item, leaving the store empty
    /// </summary>
    void Clear();
}