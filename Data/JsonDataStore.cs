using EventRegistrationsApi.Models;
using System.Text.Json;

namespace EventRegistrationsApi.Data;

public class JsonDataStore
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public JsonDataStore(IConfiguration config)
    {
        _filePath = config.GetValue<string>("DataFilePath") ?? "data.json";
        EnsureFileExists();
    }

    public async Task<T> ExecuteAsync<T>(Func<DataStore, T> operation)
    {
        await _lock.WaitAsync();
        try
        {
            var store = Load();
            var result = operation(store);
            Save(store);
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }


    public async Task<T> QueryAsync<T>(Func<DataStore, T> query)
    {
        await _lock.WaitAsync();
        try
        {
            return query(Load());
        }
        finally
        {
            _lock.Release();
        }
    }



    private DataStore Load()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<DataStore>(json, JsonOpts) ?? new DataStore();
    }

    private void Save(DataStore store)
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(store, JsonOpts));
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, JsonSerializer.Serialize(new DataStore(), JsonOpts));
    }
}

public class DataStore
{
    public List<Event> Events { get; set; } = [];
    public List<Registration> Registrations { get; set; } = [];
}
