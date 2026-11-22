namespace music_library.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using music_library.Models;

public static class LibraryStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string DefaultPath => Path.Combine(Directory.GetCurrentDirectory(), "library.json");

    public static async Task SaveAsync(Library library, string path)
    {
        ArgumentNullException.ThrowIfNull(library);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var items = new List<Audio>(library);

        await using FileStream stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, items, Options);
    }

    public static async Task<List<Audio>?> LoadAsync(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
        {
            return null;
        }

        await using FileStream stream = File.OpenRead(path);
        var items = await JsonSerializer.DeserializeAsync<List<Audio?>>(stream, Options);

        if (items is null)
        {
            return new List<Audio>();
        }

        var result = new List<Audio>(items.Count);

        foreach (Audio? audio in items)
        {
            if (audio is null)
            {
                continue;
            }

            Audio.EnsureIdCounterAtLeast(audio.Id);
            result.Add(audio);
        }

        return result;
    }
}
