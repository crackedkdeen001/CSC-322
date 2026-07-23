using BankApp.interfaces;
using BankApp.utils;

namespace BankApp.repository;

/// <summary>
/// A repository that logic shared by every repository.
/// </summary>
/// <typeparam name="T">The type of item stored, which must have an id</typeparam>
/// <param name="fileName">The name of the JSON file the items are stored in</param>
public class JsonRepository<T>(string fileName) : IRepository<T>
    where T: IHasID
{
    private readonly IFileHandler<T> _fileHandler = new JsonFileHandler<T>(fileName);

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every item in the file, or an empty list if there are none
    /// </summary>
    /// <returns>Every item in the file</returns>
    public List<T> GetAll()
    {
        return _fileHandler.LoadItems();
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the item with the given id, or null if no item has it
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>The matching item, or null</returns>
    public T? GetById(int id)
    {
        var item = GetAll().FirstOrDefault(item => item.Id == id);
        return item;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: gives the item the next free id, appends it and saves; the number of items grows by one
    /// </summary>
    /// <param name="item">The item to add</param>
    public void Add(T item)
    {
        var items = GetAll();
        item.Id = _getNextId();
        items.Add(item);
        Save(items);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: replaces the stored item sharing updatedItem's id and returns it, or returns null and leaves
    ///          the file unchanged if no item matches
    /// </summary>
    /// <param name="updatedItem">The new version of the item, matched by id</param>
    /// <returns>The updated item, or null if no item had that id</returns>
    public T? Update(T updatedItem)
    {
        var items = GetAll();
        int idx = items.FindIndex(item => item.Id == updatedItem.Id);

        if (idx != -1)
        {
            items[idx] = updatedItem;
            Save(items);

            return updatedItem;
        }
        return default;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: removes the item with the given id and returns it, or returns null and leaves the file
    ///          unchanged if no item matches
    /// </summary>
    /// <param name="id">The id of the item to remove</param>
    /// <returns>The removed item, or null if no item had that id</returns>
    public T? Delete(int id)
    {
        var items = GetAll();
        var deleted = items.FirstOrDefault(item => item.Id == id);
        if (deleted is not null)
        {
            items.Remove(deleted);
            Save(items);
        }
        return deleted;
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: replaces the file's contents with exactly the given items
    /// </summary>
    /// <param name="items">The items to save</param>
    public void Save(List<T> items)
    {
        _fileHandler.SaveItems(items);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: removes every item, leaving the file empty
    /// </summary>
    public void Clear()
    {
        _fileHandler.Clear();
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns 1 for an empty file, otherwise one more than the largest id currently stored.
    /// </summary>
    /// <returns>The id to give the next item added</returns>
    private int _getNextId() => GetAll().Count == 0 ? 1 : GetAll().Max(item => item.Id) + 1;
}