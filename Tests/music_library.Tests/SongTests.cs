using music_library.Models;

namespace music_library.Tests;

public class SongTests
{
    [Fact]
    public void ToString_FormatsAsAuthorDashTitle()
    {
        var song = new Song("Title", "Author", 238, "/music/song.mp3", PlaybackState.Stopped);

        Assert.Equal("Author - Title", song.ToString());
    }

    [Fact]
    public void ToString_DoesntStripMp3ExtensionFromTheTitle()
    {
        var song = new Song("Title.mp3", "Author", 238, "/music/song.mp3", PlaybackState.Stopped);

        Assert.Equal("Author - Title.mp3", song.ToString());
    }

    [Fact]
    public void CopyConstructor_CopiesEveryField()
    {
        var original = new Song("Title", "Author", 238, "/music/song.mp3", PlaybackState.Playing)
        {
            Album = "Test Album",
            Genre = Genre.Jazz,
            Year = 1999,
            PlayCount = 4
        };

        var copy = new Song(original);

        Assert.Equal(original.Title, copy.Title);
        Assert.Equal(original.Author, copy.Author);
        Assert.Equal(original.Album, copy.Album);
        Assert.Equal(original.Genre, copy.Genre);
        Assert.Equal(original.Year, copy.Year);
        Assert.Equal(original.Duration, copy.Duration);
        Assert.Equal(original.FilePath, copy.FilePath);
        Assert.Equal(original.State, copy.State);
        Assert.Equal(original.PlayCount, copy.PlayCount);
        Assert.NotSame(original, copy);
    }

    [Fact]
    public void Constructor_WithoutMetadata_UsesPlaceholderValues()
    {
        var song = new Song("Title", "Author", 238, "/music/song.mp3", PlaybackState.Stopped);

        Assert.Equal("Unknown Album", song.Album);
        Assert.Equal(Genre.Other, song.Genre);
        Assert.Equal(0, song.Year);
    }

    [Fact]
    public void GetFilePath_ReturnsThePathGivenToTheConstructor()
    {
        var song = new Song("Title", "Author", 238, "/music/song.mp3", PlaybackState.Stopped);

        Assert.Equal("/music/song.mp3", song.getFilePath());
    }

    [Fact]
    public void Id_IsAutoAssignedWithTrkPrefix()
    {
        var song = new Song("Title", "Author", 238, "/music/song.mp3", PlaybackState.Stopped);

        Assert.Matches(@"^trk-\d{3,}$", song.Id);
    }

    [Fact]
    public void Id_IsUniqueAcrossInstances()
    {
        var first = new Song("Title", "Author", 238, "/music/first.mp3", PlaybackState.Stopped);
        var second = new Song("Title", "Author", 238, "/music/second.mp3", PlaybackState.Stopped);

        Assert.NotEqual(first.Id, second.Id);
    }
}
