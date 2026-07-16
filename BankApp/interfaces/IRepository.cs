using System.Data;

namespace BankApp.interfaces;

public interface IRepository<T>
    where T: IHasID
{
    List<T> GetAll();
    T? GetById(int id);
    void Add(T item);
    T? Delete(int id);
}