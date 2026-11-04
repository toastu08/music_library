using music_library.Models;

namespace music_library.Tests;

public class ActivityLogTests
{
    [Fact]
    public void OnAudioAdded_AppendsAnEntry()
    {
        var log = new ActivityLog();
        var song = new Song("Title", "Author", 100, "/music/title.mp3", PlaybackState.Stopped);

        log.OnAudioAdded(null, new AudioAddedEventArgs(song, 1));

        var entry = Assert.Single(log.Entries);
        Assert.Contains("Author - Title", entry);
        Assert.Contains("1 in library", entry);
    }

    [Fact]
    public void LibraryAdd_IsRecordedByTheLog()
    {
        var library = new Library();
        var log = new ActivityLog();
        library.AudioAdded += log.OnAudioAdded;

        library.AddSongFromPath("/music/Author - Title.mp3");

        Assert.Single(log.Entries);
    }

    [Fact]
    public void Clear_EmptiesTheLog()
    {
        var log = new ActivityLog();
        log.Write("something");

        log.Clear();

        Assert.Empty(log.Entries);
    }
}
