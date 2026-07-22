using System.Text.Json;
using System.Text.Json.Serialization;
using BankApp.interfaces;

namespace BankApp.utils;

/// <summary>
/// Reads and writes a list of items to a JSON file on disk
/// </summary>
/// <param name="filePath">The path of the file to read from and write to</param>
public class JsonFileHandler<T>(string fileName) : IFileHandler<T>
    where T : IHasID
{
    private readonly JsonSerializerOptions _options = new() {
        WriteIndented = true,
        IncludeFields = true, 
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    private static readonly string DatabaseDirectory =
        Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent?.Parent?.Parent?.FullName, "database");
    
    /// <summary>
    /// The full path of the file this handler reads from and writes to
    /// </summary>
    public string FilePath { get; } = BuildFilePath(fileName);

    private static string BuildFilePath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty", fileName);
        }

        Directory.CreateDirectory(DatabaseDirectory);
        return Path.Combine(DatabaseDirectory, fileName);

    }
    /// <summary>
    /// Loads every item from the file<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns the items in the file, or an empty list if the file does not exist yet
    /// </summary>
    /// <returns>The items in the file, or an empty list if the file does not exist yet</returns>
    public List<T> LoadItems()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
            return [];
        }

        string jsonString = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return [];
        }

        List<T>? items = JsonSerializer.Deserialize<List<T>>(jsonString, _options);
        return items ?? [];
    }

    /// <summary>
    /// Saves a list of items to the file, replacing whatever was there before<br/>
    /// PreCondition: None<br/>
    /// PostCondition: The file contains exactly the given items
    /// </summary>
    /// <param name="items">The items to save</param>
    public void SaveItems(List<T> items)
    {
        string jsonString = JsonSerializer.Serialize(items, _options);
        File.WriteAllText(FilePath, jsonString);
    }

    public void Clear()
    {
        SaveItems([]);
    }
}
