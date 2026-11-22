namespace music_library.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

public sealed class Library : IReadOnlyList<Audio>, IList, INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly List<Audio> _items = new();

    public Audio this[int index] => _items[index];

    public Audio? this[string id] => _items.Find(audio => audio.Id == id);

    public int Count => _items.Count;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<AudioAddedEventArgs>? AudioAdded;

    public void Add(Audio audio){
        _items.Add(audio);
        RaiseCountChanged();
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, audio, _items.Count - 1));
        AudioAdded?.Invoke(this, new AudioAddedEventArgs(audio, _items.Count));
    }

    public bool Remove(Audio audio){
        int index = _items.IndexOf(audio);

        if (index < 0){
            return false;
        }

        _items.RemoveAt(index);
        RaiseCountChanged();
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, audio, index));
        return true;
    }

    public void Clear(){
        if (_items.Count == 0){
            return;
        }

        _items.Clear();
        RaiseCountChanged();
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public int IndexOf(Audio audio) => _items.IndexOf(audio);

    public IEnumerator<Audio> GetEnumerator(){
        foreach (Audio audio in _items){
            yield return audio;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<Audio> ByAuthor(string author){
        foreach (Audio audio in _items){
            if (audio.Author == author){
                yield return audio;
            }
        }
    }

    public void AddSongFromPath(string? path){
        if (path is null){
            Console.WriteLine("error throw exception (fix later)");

            return;
        }

        string fileName = path.Split('/')[^1];

        if (fileName.Contains(" - ")){
            string author = fileName.Split('-')[0].Trim();
            string title = fileName.Split('-')[^1].Trim().Replace(".mp3", "");
            Song song = new Song(title, author, 100, path, PlaybackState.Stopped);
            Add(song);
        }
        else{
            string title = fileName.Trim().Replace(".mp3", "");
            string author = "Unknown";
            Song song = new Song(title, author, 100, path, PlaybackState.Stopped);
            Add(song);
        }


    }

    public void ForEach(Action<Audio> action){
        ArgumentNullException.ThrowIfNull(action);

        foreach (Audio audio in _items){
            action(audio);
        }
    }

    public void DescribeAll(){
        ForEach(audio => Console.WriteLine(audio.Describe()));
    }

    private void RaiseCountChanged(){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    // Avalonia only wraps a notifying collection that also implements the non-generic IList,
    // so this is the read-only view used by the UI. Changes still go through Add/Remove/Clear.
    bool IList.IsReadOnly => true;
    bool IList.IsFixedSize => true;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;

    object? IList.this[int index]
    {
        get => _items[index];
        set => throw new NotSupportedException("Library can only be changed through Add and Remove.");
    }

    int IList.Add(object? value) => throw new NotSupportedException("Library can only be changed through Add and Remove.");
    void IList.Clear() => throw new NotSupportedException("Library can only be changed through Add and Remove.");
    void IList.Insert(int index, object? value) => throw new NotSupportedException("Library can only be changed through Add and Remove.");
    void IList.Remove(object? value) => throw new NotSupportedException("Library can only be changed through Add and Remove.");
    void IList.RemoveAt(int index) => throw new NotSupportedException("Library can only be changed through Add and Remove.");

    bool IList.Contains(object? value) => value is Audio audio && _items.Contains(audio);
    int IList.IndexOf(object? value) => value is Audio audio ? _items.IndexOf(audio) : -1;
    void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);
}
