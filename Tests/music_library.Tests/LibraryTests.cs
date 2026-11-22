using System.Collections.Specialized;
using Avalonia.Controls;
using music_library.Models;

namespace music_library.Tests;

public class LibraryTests
{
    [Fact]
    public void AddSongFromPath_SplitsAuthorAndTitleOnDash()
    {
        var library = new Library();

        library.AddSongFromPath("/home/toastu/Music/Author - Title.mp3");

        var song = Assert.Single(library);
        Assert.Equal("Author", song.Author);
        // .mp3 extension is kept by the AddSongFromPath method, Song.toString() is what removes it (idk why i implement it like that tbh)
        //update: changed, it was too weird that way
        Assert.Equal("Title", song.Title);
    }

    [Fact]
    public void AddSongFromPath_WithoutSpacedDash_TreatsWholeFileNameAsTitle()
    {
        var library = new Library();

        library.AddSongFromPath("/home/toastu/Music/Author-Title.mp3");

        var song = Assert.Single(library);
        Assert.Equal("Unknown", song.Author);
        Assert.Equal("Author-Title", song.Title);
        //this is because i have a yt downloader written in python that formats downloads in a specific way btw
    }

    [Fact]
    public void AddSongFromPath_WithoutAnyDash_UsesUnknownAuthor()
    {
        var library = new Library();

        library.AddSongFromPath("/home/toastu/Music/mystery-track.mp3");

        var song = Assert.Single(library);
        Assert.Equal("Unknown", song.Author);
        Assert.Equal("mystery-track", song.Title);
    }

    [Fact]
    public void AddSongFromPath_WithNullPath_AddsNothing()
    {
        var library = new Library();

        library.AddSongFromPath(null);

        Assert.Empty(library);
    }

    [Fact]
    public void AddSongFromPath_UsesOnlyTheLastPathSegment()
    {
        var library = new Library();

        library.AddSongFromPath("/home/toastu/Music/DNB/Xypherdon - Not Today.mp3");

        var song = Assert.Single(library);
        Assert.DoesNotContain("/", song.Author);
        Assert.DoesNotContain("/", song.Title);
        Assert.Equal("Xypherdon", song.Author);
    }

    [Fact]
    public void AddSongFromPath_DefaultsToPlaceholderDurationAndStoppedState()
    {
        var library = new Library();

        library.AddSongFromPath("/home/toastu/Music/Author - Title.mp3");

        var song = Assert.Single(library);
        Assert.Equal(TimeSpan.FromSeconds(100), song.Duration);
        Assert.Equal(PlaybackState.Stopped, song.State);
        Assert.Equal("/home/toastu/Music/Author - Title.mp3", song.FilePath);
    }

    [Fact]
    public void AddSongFromPath_AppendsSongsInOrder()
    {
        var library = new Library();

        library.AddSongFromPath("/music/Artist A - First.mp3");
        library.AddSongFromPath("/music/Artist B - Second.mp3");

        Assert.Equal(2, library.Count);
        Assert.Equal("First", library[0].Title);
        Assert.Equal("Second", library[1].Title);
    }

    [Fact]
    public void IndexerByInt_ReturnsTheItemAtThatIndex()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");
        library.AddSongFromPath("/music/Artist B - Second.mp3");

        Assert.Equal("First", library[0].Title);
        Assert.Equal("Second", library[1].Title);
    }

    [Fact]
    public void IndexerByInt_OutOfRange_Throws()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");

        Assert.Throws<ArgumentOutOfRangeException>(() => _ = library[1]);
    }

    [Fact]
    public void IndexerById_ReturnsTheMatchingItem()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");
        var first = library[0];

        Assert.Same(first, library[first.Id]);
    }

    [Fact]
    public void IndexerById_UnknownId_ReturnsNull()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");

        Assert.Null(library["trk-does-not-exist"]);
    }

    [Fact]
    public void Foreach_YieldsItemsInInsertionOrder()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");
        library.AddSongFromPath("/music/Artist B - Second.mp3");
        library.AddSongFromPath("/music/Artist C - Third.mp3");

        var titles = new List<string>();
        foreach (var audio in library)
        {
            titles.Add(audio.Title);
        }

        Assert.Equal(new[] { "First", "Second", "Third" }, titles);
    }

    [Fact]
    public void ByAuthor_ReturnsOnlyThatAuthorsTracks()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");
        library.AddSongFromPath("/music/Artist B - Second.mp3");
        library.AddSongFromPath("/music/Artist A - Third.mp3");

        var titles = library.ByAuthor("Artist A").Select(audio => audio.Title).ToList();

        Assert.Equal(new[] { "First", "Third" }, titles);
    }

    [Fact]
    public void ByAuthor_IsLazy()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");

        var query = library.ByAuthor("Artist A");
        library.AddSongFromPath("/music/Artist A - Second.mp3");

        Assert.Equal(2, query.Count());
    }

    [Fact]
    public void Remove_RemovesTheItem()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Artist A - First.mp3");
        var audio = library[0];

        Assert.True(library.Remove(audio));
        Assert.Empty(library);
        Assert.False(library.Remove(audio));
    }

    [Fact]
    public void Clear_RemovesEverythingAndRaisesReset()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Author - First.mp3");
        library.AddSongFromPath("/music/Author - Second.mp3");
        var changes = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)library).CollectionChanged +=
            (_, e) => changes.Add(e.Action);

        library.Clear();

        Assert.Empty(library);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, changes);
    }

    [Fact]
    public void AddSongFromPath_NotifiesCollectionChanged()
    {
        var library = new Library();
        var changes = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)library).CollectionChanged +=
            (_, e) => changes.Add(e.Action);

        library.AddSongFromPath("/music/Author - Title.mp3");

        Assert.Equal(new[] { NotifyCollectionChangedAction.Add }, changes);
    }

    // Avalonia throws "Collection implements INotifyCollectionChanged but not IList"
    // unless the collection also implements the non-generic IList. This is how the ListBox sees it.
    [Fact]
    public void Library_CanBeWrappedByAvaloniaItemsSourceView()
    {
        var library = new Library();
        library.AddSongFromPath("/music/Author - First.mp3");

        var view = ItemsSourceView.GetOrCreate(library);

        Assert.Single(view);

        var changes = 0;
        view.CollectionChanged += (_, _) => changes++;

        library.AddSongFromPath("/music/Author - Second.mp3");

        Assert.Equal(2, view.Count);
        Assert.Equal(1, changes);
        Assert.Equal("Author - Second", view[1]?.ToString());
    }
}
