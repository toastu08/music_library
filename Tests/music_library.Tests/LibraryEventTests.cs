using music_library.Models;

namespace music_library.Tests;

public class LibraryEventTests
{
    private static Song NewSong(string title) =>
        new(title, "Author", 100, $"/music/{title}.mp3", PlaybackState.Stopped);

    [Fact]
    public void Add_RaisesAudioAddedWithTheAddedItem()
    {
        var library = new Library();
        AudioAddedEventArgs? received = null;
        library.AudioAdded += (_, e) => received = e;

        var song = NewSong("First");
        library.Add(song);

        Assert.NotNull(received);
        Assert.Same(song, received.Audio);
        Assert.Equal(1, received.Count);
    }

    [Fact]
    public void Add_NotifiesBothSubscribersInSubscriptionOrder()
    {
        var library = new Library();
        var calls = new List<string>();
        library.AudioAdded += (_, _) => calls.Add("log");
        library.AudioAdded += (_, _) => calls.Add("count");

        library.Add(NewSong("First"));

        Assert.Equal(new[] { "log", "count" }, calls);
    }

    [Fact]
    public void Add_WithNoSubscribers_DoesNotThrow()
    {
        var library = new Library();

        Assert.Null(Record.Exception(() => library.Add(NewSong("First"))));
    }

    [Fact]
    public void Remove_DoesNotRaiseAudioAdded()
    {
        var library = new Library();
        var song = NewSong("First");
        library.Add(song);

        var raised = 0;
        library.AudioAdded += (_, _) => raised++;

        library.Remove(song);

        Assert.Equal(0, raised);
    }

    [Fact]
    public void UnsubscribedHandler_IsNotCalled()
    {
        var library = new Library();
        var calls = 0;
        EventHandler<AudioAddedEventArgs> handler = (_, _) => calls++;

        library.AudioAdded += handler;
        library.AudioAdded -= handler;

        library.Add(NewSong("First"));

        Assert.Equal(0, calls);
    }

    [Fact]
    public void AudioAddedCount_ReflectsTheSizeAfterEachAdd()
    {
        var library = new Library();
        var counts = new List<int>();
        library.AudioAdded += (_, e) => counts.Add(e.Count);

        library.Add(NewSong("First"));
        library.Add(NewSong("Second"));
        library.Add(NewSong("Third"));

        Assert.Equal(new[] { 1, 2, 3 }, counts);
    }

    [Fact]
    public void ForEach_CallsTheActionForEveryItemInOrder()
    {
        var library = new Library();
        library.Add(NewSong("First"));
        library.Add(NewSong("Second"));

        var titles = new List<string>();
        library.ForEach(audio => titles.Add(audio.Title));

        Assert.Equal(new[] { "First", "Second" }, titles);
    }

    [Fact]
    public void ForEach_AcceptsALocalFunction()
    {
        var library = new Library();
        library.Add(NewSong("First"));
        library.Add(NewSong("Second"));

        var titles = new List<string>();
        void Record(Audio audio) => titles.Add(audio.Title);

        library.ForEach(Record);

        Assert.Equal(new[] { "First", "Second" }, titles);
    }

    [Fact]
    public void ForEach_WithNullAction_Throws()
    {
        var library = new Library();

        Assert.Throws<ArgumentNullException>(() => library.ForEach(null!));
    }
}
