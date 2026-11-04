namespace music_library.Models;

using System;
using System.Collections.Generic;

public sealed class ActivityLog
{
    private readonly List<string> _entries = new();

    public IReadOnlyList<string> Entries => _entries;

    public void OnAudioAdded(object? sender, AudioAddedEventArgs e){
        Write($"Added: {e.Audio.Author} - {e.Audio.Title} ({e.Count} in library)");
    }

    public void Write(string message){
        _entries.Add(message);
        Console.WriteLine(message);
    }

    public void Clear() => _entries.Clear();
}
