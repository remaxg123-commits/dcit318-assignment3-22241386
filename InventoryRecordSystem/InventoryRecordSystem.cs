using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// ---- Marker interface ----
public interface IInventoryEntity
{
    int Id { get; }
}

// ---- Immutable record ----
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// ---- Generic logger ----
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log;
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log = items ?? new List<T>();
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: file '{_filePath}' was not found.");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error reading file contents: {ex.Message}");
        }
    }
}

// ---- Integration layer ----
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Office Chair", 20, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Desk Lamp", 35, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Whiteboard", 10, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Printer Paper", 100, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:d}");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new InventoryApp();
        app.SeedSampleData();
        app.SaveData();

        // Simulate a new session by creating a fresh app instance and loading from disk
        var newSessionApp = new InventoryApp();
        newSessionApp.LoadData();
        newSessionApp.PrintAllItems();
    }
}
