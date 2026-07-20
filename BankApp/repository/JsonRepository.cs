using BankApp.interfaces;
using BankApp.utils;

namespace BankApp.repository;

/// <summary>
/// A repository containing general methods that both the Account and Transaction Repository share
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonRepository<T>(string fileName) : IRepository<T>
    where T: IHasID
{
    private IFileHandler<T> _fileHandler = new JsonFileHandler<T>(fileName);


    /// <summary>
    /// Gets all objects from a specified JSON file <br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns a list of objects or an empty list if the JSON file is empty
    /// </summary>
    /// <returns>List of objects or an  empty list</returns>
    public List<T> GetAll()
    {
        return _fileHandler.LoadItems();
    }

    /// <summary>
    /// Gets an object by the ID<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns the object if found; null if it doesn't exist
    /// </summary>
    /// <param name="id">The ID of the object</param>
    /// <returns>The object if found, null if it doesn't exist</returns>
    public T? GetById(int id)
    {
        var item = GetAll().FirstOrDefault(item => item.Id == id);
        return item;
    }

    /// <summary>
    /// Adds an object to the database<br/>
    /// PreCondition: The objects must exist<br/>
    /// PostCondition: The number of objects in the database increases by 1
    /// </summary>
    /// <param name="item">Object to be added</param>
    public void Add(T item)
    {
        var items = GetAll();
        item.Id = _getNextId();
        items.Add(item);
        Save(items);
    }


    /// <summary>
    /// Updates an item in the database<br/>
    /// PreCondition: The original item to be updated must exist
    /// PostCondition: The original item's contents have been updated to the new item's contents
    /// </summary>
    /// 
    /// <param name="updatedItem"></param>
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
    /// Deletes an object from the database and returns it<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Number of objects in the database is reduced by 1 if the object is found and returns the object;
    ///                No-Op if the object is not found and null is returned.
    /// </summary>
    /// <param name="id">The ID of the object</param>
    /// <returns>Deleted object if it exists, null if not.</returns>
    public T? Delete(int id)
    {
        var items = GetAll();
        var deleted = GetById(id);
        if (deleted is not null)
        {
            items.Remove(deleted);
            Save(items);
        }
        return deleted;
    }

    /// <summary>
    /// Saves a list of objects into the database<br/>
    /// Precondition: None<br/>
    /// Postcondition: The database is replaced with the new list of values
    /// </summary>
    /// <param name="items">List of values to save</param>
    public void Save(List<T> items)
    {
        _fileHandler.SaveItems(items);
    }

    private int _getNextId() => GetAll().Count == 0 ? 1 : GetAll().Count + 1;
}