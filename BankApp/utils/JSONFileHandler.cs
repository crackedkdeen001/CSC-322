using BankApp.interfaces;

namespace BankApp.utils;
using System.Text.Json;


public class JSONFileHandler<T>(string fileName)
    where T: IHasID
{
    private JsonSerializerOptions _options = new() { WriteIndented = true };
    private string filePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName + "/" + fileName;
    
    public List<T> LoadItems()
    {
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            List<T> items = JsonSerializer.Deserialize<List<T>>(jsonString, _options);

            return items ?? new List<T>();
        }

        return new List<T>();
    }

    public void SaveItems(List<T> items)
    {
        string jsonString = JsonSerializer.Serialize(items, _options);
        File.WriteAllText(filePath, jsonString);
    }
}