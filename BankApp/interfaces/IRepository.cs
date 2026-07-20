namespace BankApp.interfaces;

public interface IRepository<T>
    where T: IHasID
{
    List<T> GetAll();
    T? GetById(int id);
    void Add(T item);
    T? Update(T updatedItem);
    T? Delete(int id);
    void Save(List<T> items);
}
