namespace music_library.Models;

using System;
using System.Collections;
using System.Collections.Generic;

public sealed class RecentlyPlayed<T> : IEnumerable<T> where T : Audio
{
    private readonly List<T> _items = new();

    public RecentlyPlayed(int capacity = 10){
        if (capacity <= 0){
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
        }

        Capacity = capacity;
    }

    public int Capacity { get; }

    public int Count => _items.Count;

    public T this[int index] => _items[index];

    public void Record(T audio){
        ArgumentNullException.ThrowIfNull(audio);

        _items.Remove(audio);
        _items.Insert(0, audio);

        if (_items.Count > Capacity){
            _items.RemoveAt(_items.Count - 1);
        }
    }

    public IEnumerator<T> GetEnumerator(){
        foreach (T audio in _items){
            yield return audio;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
