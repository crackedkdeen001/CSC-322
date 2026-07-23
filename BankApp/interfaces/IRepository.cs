namespace BankApp.interfaces;

/// <summary>
/// A store of items that can be looked up, added, changed and removed. Each item carries its own id.
/// </summary>
/// <typeparam name="T">The type of item stored, which must have an id</typeparam>
public interface IRepository<T>
    where T: IHasID
{
    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every item in the store, or an empty list if there are none
    /// </summary>
    /// <returns>Every item in the store</returns>
    List<T> GetAll();

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the item with the given id, or null if no item has it
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>The matching item, or null</returns>
    T? GetById(int id);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the store<br/>
    /// effects: gives the item a fresh id and saves it
    /// </summary>
    /// <param name="item">The item to add</param>
    void Add(T item);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the store<br/>
    /// effects: replaces the stored item that shares updatedItem's id and returns it, or returns null if
    ///          none matches
    /// </summary>
    /// <param name="updatedItem">The new version of the item, matched by id</param>
    /// <returns>The updated item, or null if no item had that id</returns>
    T? Update(T updatedItem);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the store<br/>
    /// effects: removes the item with the given id and returns it, or returns null if none matches
    /// </summary>
    /// <param name="id">The id of the item to remove</param>
    /// <returns>The removed item, or null if no item had that id</returns>
    T? Delete(int id);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the store<br/>
    /// effects: replaces the entire contents of the store with the given items
    /// </summary>
    /// <param name="items">The items to store</param>
    void Save(List<T> items);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the store<br/>
    /// effects: removes every item, leaving the store empty
    /// </summary>
    void Clear();
}