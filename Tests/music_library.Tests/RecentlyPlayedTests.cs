using music_library.Models;

namespace music_library.Tests;

public class RecentlyPlayedTests
{
    private static Song NewSong(string title) =>
        new(title, "Author", 100, $"/music/{title}.mp3", PlaybackState.Stopped);

    [Fact]
    public void IsEmptyByDefault()
    {
        var recent = new RecentlyPlayed<Audio>();

        Assert.Empty(recent);
        Assert.Equal(0, recent.Count);
        Assert.Equal(10, recent.Capacity);
    }

    [Fact]
    public void KeepsNewestFirst()
    {
        var recent = new RecentlyPlayed<Audio>();
        var first = NewSong("First");
        var second = NewSong("Second");

        recent.Record(first);
        recent.Record(second);

        Assert.Same(second, recent[0]);
        Assert.Same(first, recent[1]);
    }

    [Fact]
    public void ReplayingMovesItemToFrontWithoutDuplicating()
    {
        var recent = new RecentlyPlayed<Audio>();
        var first = NewSong("First");
        var second = NewSong("Second");

        recent.Record(first);
        recent.Record(second);
        recent.Record(first);

        Assert.Equal(2, recent.Count);
        Assert.Same(first, recent[0]);
        Assert.Same(second, recent[1]);
    }

    [Fact]
    public void DropsTheOldestItemPastCapacity()
    {
        var recent = new RecentlyPlayed<Audio>(capacity: 3);
        var songs = Enumerable.Range(1, 4).Select(i => NewSong($"Song {i}")).ToList();

        foreach (var song in songs)
        {
            recent.Record(song);
        }

        Assert.Equal(3, recent.Count);
        Assert.DoesNotContain(songs[0], recent);
        Assert.Same(songs[3], recent[0]);
        Assert.Same(songs[1], recent[2]);
    }

    [Fact]
    public void EnumeratesNewestFirst()
    {
        var recent = new RecentlyPlayed<Audio>();
        var first = NewSong("First");
        var second = NewSong("Second");

        recent.Record(first);
        recent.Record(second);

        Assert.Equal(new[] { second, first }, recent.ToArray());
    }

    [Fact]
    public void RejectsNonPositiveCapacity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecentlyPlayed<Audio>(capacity: 0));
    }
}
