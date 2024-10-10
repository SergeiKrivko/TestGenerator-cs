using System.Text.Json;
using System.Xml;

namespace Shared.Utils;

public class SettingsSection
{
    private Dictionary<string, string?> _dictionary;
    public bool IsGlobal { get; } = false;
    public string? Name { get; }

    public delegate void ChangeHandler();

    public event ChangeHandler? Changed;

    protected SettingsSection(string name, Dictionary<string, string?> dictionary)
    {
        Name = name;
        _dictionary = dictionary;
    }

    protected static SettingsSection Empty(string name)
    {
        return new SettingsSection(name, []);
    }

    protected static SettingsSection FromDictionary(string name, Dictionary<string, string?> dictionary)
    {
        return new SettingsSection(name, dictionary);
    }

    public void Set(string key, string? value)
    {
        _dictionary[key] = value;
        Changed?.Invoke();
    }

    public void Set(string key, object? obj)
    {
        Set(key, JsonSerializer.Serialize(obj));
    }

    public void Remove(string key)
    {
        _dictionary.Remove(key);
    }

    public string? Get(string key)
    {
        if (_dictionary.TryGetValue(key, out var res))
            return res;
        return null;
    }

    public string Get(string key, string defaultValue)
    {
        if (_dictionary.TryGetValue(key, out var res))
            return res ?? defaultValue;
        return defaultValue;
    }

    public T Get<T>(string key, T defaultValue)
    {
        var res = Get<T>(key);
        return res ?? defaultValue;
    }

    public T? Get<T>(string key)
    {
        var str = Get(key);
        if (str == null)
            return default;
        try
        {
            return JsonSerializer.Deserialize<T>(str);
        }
        catch (JsonException e)
        {
            return default;
        }
    }

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator() => _dictionary.GetEnumerator();
}