using music_library.Models;
using music_library.Services;

namespace music_library.Tests;

public class LibraryQueriesTests
{
    private static Song NewSong(
        string title,
        string author,
        int durationSeconds,
        Genre genre,
        int year,
        string album,
        int playCount = 0)
    {
        return new Song(
            title,
            author,
            durationSeconds,
            $"/music/{title}.mp3",
            PlaybackState.Stopped,
            genre,
            year,
            album)
        {
            PlayCount = playCount
        };
    }

    [Fact]
    public void MostPlayedArtist_SumsPlayCountsAndPicksTheTopArtist()
    {
        var library = new Library();
        library.Add(NewSong("A1", "Artist A", 100, Genre.Rock, 1990, "Album A", 2));
        library.Add(NewSong("A2", "Artist A", 100, Genre.Rock, 1990, "Album A", 3));
        library.Add(NewSong("B1", "Artist B", 100, Genre.Pop, 2000, "Album B", 4));

        var result = LibraryQueries.MostPlayedArtist(library);

        Assert.Contains("Artist A — 5 plays", result.Body);
        Assert.Contains("Artist B — 4 plays", result.Body);
    }

    [Fact]
    public void MostPlayedArtist_WithNoPlays_ReturnsFriendlyMessage()
    {
        var library = new Library();
        library.Add(NewSong("A1", "Artist A", 100, Genre.Rock, 1990, "Album A"));

        var result = LibraryQueries.MostPlayedArtist(library);

        Assert.Contains("No plays recorded yet.", result.Body);
    }

    [Fact]
    public void HoursPerGenre_GroupsAndSumsDurations()
    {
        var library = new Library();
        library.Add(NewSong("R1", "Artist", 100, Genre.Rock, 1990, "Album"));
        library.Add(NewSong("R2", "Artist", 200, Genre.Rock, 1990, "Album"));
        library.Add(NewSong("J1", "Artist", 300, Genre.Jazz, 1990, "Album"));

        var result = LibraryQueries.HoursPerGenre(library);

        Assert.Contains("Rock — 5m (2 tracks)", result.Body);
        Assert.Contains("Jazz — 5m (1 track)", result.Body);
    }

    [Fact]
    public void NeverPlayed_ReturnsOnlyTracksWithZeroPlays()
    {
        var library = new Library();
        library.Add(NewSong("Spun", "Artist", 100, Genre.Rock, 1990, "Album", 3));
        library.Add(NewSong("Silent", "Artist", 100, Genre.Rock, 1990, "Album"));

        var result = LibraryQueries.NeverPlayed(library);

        Assert.Contains("Silent", result.Body);
        Assert.DoesNotContain("Spun", result.Body);
    }

    [Fact]
    public void NeverPlayed_WhenEveryTrackHasBeenPlayed_ReturnsFriendlyMessage()
    {
        var library = new Library();
        library.Add(NewSong("Spun", "Artist", 100, Genre.Rock, 1990, "Album", 1));

        var result = LibraryQueries.NeverPlayed(library);

        Assert.Contains("Every track has been played at least once.", result.Body);
    }

    [Fact]
    public void LongestAlbum_GroupsByAlbumAndSumsDurations()
    {
        var library = new Library();
        library.Add(NewSong("A1", "Artist", 100, Genre.Rock, 1990, "Album A"));
        library.Add(NewSong("A2", "Artist", 200, Genre.Rock, 1990, "Album A"));
        library.Add(NewSong("B1", "Artist", 250, Genre.Pop, 2000, "Album B"));

        var result = LibraryQueries.LongestAlbum(library);

        Assert.Contains("Album A — 5m (2 tracks)", result.Body);
        Assert.Contains("Album B — 4m 10s (1 track)", result.Body);
    }

    [Fact]
    public void TopDecade_GroupsByDecadeAndIgnoresUnknownYears()
    {
        var library = new Library();
        library.Add(NewSong("A", "Artist", 100, Genre.Rock, 1990, "Album"));
        library.Add(NewSong("B", "Artist", 100, Genre.Rock, 1995, "Album"));
        library.Add(NewSong("C", "Artist", 100, Genre.Pop, 2001, "Album"));
        library.Add(NewSong("D", "Artist", 100, Genre.Pop, 0, "Album"));

        var result = LibraryQueries.TopDecade(library);

        Assert.Contains("1990s — 2 tracks", result.Body);
        Assert.Contains("2000s — 1 track", result.Body);
        Assert.Equal(2, result.Body.Split(Environment.NewLine).Length);
    }

    [Fact]
    public void Queries_HandleAnEmptyLibrary()
    {
        var library = new Library();

        Assert.Contains("No plays recorded yet.", LibraryQueries.MostPlayedArtist(library).Body);
        Assert.Contains("No tracks in the library.", LibraryQueries.HoursPerGenre(library).Body);
        Assert.Contains("No tracks in the library.", LibraryQueries.NeverPlayed(library).Body);
        Assert.Contains("No tracks in the library.", LibraryQueries.LongestAlbum(library).Body);
        Assert.Contains("No release years recorded.", LibraryQueries.TopDecade(library).Body);
    }
}
