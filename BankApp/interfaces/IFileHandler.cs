namespace BankApp.interfaces;

public interface IFileHandler<T>
{
    List<T> LoadItems();
    void SaveItems(List<T> items);

    void Clear();
}