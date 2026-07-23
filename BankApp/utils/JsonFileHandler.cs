using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using BankApp.interfaces;

namespace BankApp.utils;

/// <summary>
/// Reads and writes a list of items to a JSON file on disk, under a shared database directory.
/// </summary>
/// <param name="fileName">The name of the JSON file to read from and write to</param>
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
    private string FilePath { get; } = BuildFilePath(fileName);

    /// <summary>
    /// requires: fileName ends in ".json" and is not blank<br/>
    /// modifies: the file system (creates the database directory if it is absent)<br/>
    /// effects: returns the full path of the file under the database directory; throws ArgumentException if
    ///          fileName is not a ".json" name or is blank
    /// </summary>
    /// <param name="fileName">The name of the JSON file</param>
    /// <returns>The full path of the file under the database directory</returns>
    /// <exception cref="ArgumentException">thrown if fileName is not a ".json" name or is blank</exception>
    private static string BuildFilePath(string fileName)
    {
        string jsonPattern = @"\b\w+\.json$";
        if (!Regex.IsMatch(fileName, jsonPattern))
        {
            throw new ArgumentException("Provided string is not a json file", fileName);
        }
        
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty", fileName);
        }

        Directory.CreateDirectory(DatabaseDirectory);
        return Path.Combine(DatabaseDirectory, fileName);

    }
    /// <summary>
    /// requires: none<br/>
    /// modifies: the file (creates it empty if it does not exist yet)<br/>
    /// effects: returns every item in the file, or an empty list if the file is missing or empty
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
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: replaces the file's contents with exactly the given items
    /// </summary>
    /// <param name="items">The items to save</param>
    public void SaveItems(List<T> items)
    {
        string jsonString = JsonSerializer.Serialize(items, _options);
        File.WriteAllText(FilePath, jsonString);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the file<br/>
    /// effects: removes every item, leaving the file holding an empty list
    /// </summary>
    public void Clear()
    {
        SaveItems([]);
    }
}
