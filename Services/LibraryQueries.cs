namespace music_library.Services;

using System;
using System.Linq;
using music_library.Models;

public static class LibraryQueries
{
    public static QueryResult MostPlayedArtist(Library library)
    {
        var ranking = library
            .GroupBy(audio => audio.Author)
            .Select(group => new
            {
                Artist = group.Key,
                Plays = group.Sum(audio => audio.PlayCount)
            })
            .Where(row => row.Plays > 0)
            .OrderByDescending(row => row.Plays)
            .ThenBy(row => row.Artist, StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .ToList();

        if (ranking.Count == 0)
        {
            return new QueryResult("Most Played Artist", "No plays recorded yet.");
        }

        string body = string.Join(
            Environment.NewLine,
            ranking.Select((row, index) => $"{index + 1}. {row.Artist} — {FormatPlayCount(row.Plays)}"));

        return new QueryResult("Most Played Artist", body);
    }

    public static QueryResult HoursPerGenre(Library library)
    {
        var rows = library
            .GroupBy(audio => audio.Genre)
            .Select(group => new
            {
                Genre = group.Key,
                Duration = TimeSpan.FromTicks(group.Sum(audio => audio.Duration.Ticks)),
                Count = group.Count()
            })
            .OrderByDescending(row => row.Duration)
            .ThenBy(row => row.Genre)
            .ToList();

        if (rows.Count == 0)
        {
            return new QueryResult("Hours Per Genre", "No tracks in the library.");
        }

        string body = string.Join(
            Environment.NewLine,
            rows.Select(row => $"{row.Genre} — {FormatDuration(row.Duration)} ({FormatTrackCount(row.Count)})"));

        return new QueryResult("Hours Per Genre", body);
    }

    public static QueryResult NeverPlayed(Library library)
    {
        if (library.Count == 0)
        {
            return new QueryResult("Tracks Never Played", "No tracks in the library.");
        }

        var tracks = library
            .Where(audio => audio.PlayCount == 0)
            .OrderBy(audio => audio.Author, StringComparer.OrdinalIgnoreCase)
            .ThenBy(audio => audio.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (tracks.Count == 0)
        {
            return new QueryResult("Tracks Never Played", "Every track has been played at least once.");
        }

        string header = tracks.Count == 1
            ? $"1 of {library.Count} tracks has never been played:"
            : $"{tracks.Count} of {library.Count} tracks have never been played:";

        string body = header + Environment.NewLine +
                      string.Join(Environment.NewLine, tracks.Select(audio => $"{audio.Author} - {audio.Title}"));

        return new QueryResult("Tracks Never Played", body);
    }

    public static QueryResult LongestAlbum(Library library)
    {
        var albums = library
            .GroupBy(audio => audio.Album)
            .Select(group => new
            {
                Album = group.Key,
                Duration = TimeSpan.FromTicks(group.Sum(audio => audio.Duration.Ticks)),
                Count = group.Count()
            })
            .OrderByDescending(row => row.Duration)
            .ThenBy(row => row.Album, StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .ToList();

        if (albums.Count == 0)
        {
            return new QueryResult("Longest Album", "No tracks in the library.");
        }

        string body = string.Join(
            Environment.NewLine,
            albums.Select((row, index) => $"{index + 1}. {row.Album} — {FormatDuration(row.Duration)} ({FormatTrackCount(row.Count)})"));

        return new QueryResult("Longest Album", body);
    }

    public static QueryResult TopDecade(Library library)
    {
        var decades = library
            .Where(audio => audio.Year > 0)
            .GroupBy(audio => audio.Year / 10 * 10)
            .Select(group => new
            {
                Decade = group.Key,
                Count = group.Count(),
                Duration = TimeSpan.FromTicks(group.Sum(audio => audio.Duration.Ticks))
            })
            .OrderByDescending(row => row.Count)
            .ThenByDescending(row => row.Duration)
            .ThenBy(row => row.Decade)
            .Take(5)
            .ToList();

        if (decades.Count == 0)
        {
            return new QueryResult("Top Decade", "No release years recorded.");
        }

        string body = string.Join(
            Environment.NewLine,
            decades.Select((row, index) => $"{index + 1}. {row.Decade}s — {FormatTrackCount(row.Count)} ({FormatDuration(row.Duration)})"));

        return new QueryResult("Top Decade", body);
    }

    private static string FormatDuration(TimeSpan duration)
    {
        int hours = (int)duration.TotalHours;
        int minutes = duration.Minutes;
        int seconds = duration.Seconds;

        if (hours > 0)
        {
            return minutes > 0 ? $"{hours}h {minutes}m" : $"{hours}h";
        }

        if (minutes > 0)
        {
            return seconds > 0 ? $"{minutes}m {seconds}s" : $"{minutes}m";
        }

        return $"{seconds}s";
    }

    private static string FormatTrackCount(int count) => count == 1 ? "1 track" : $"{count} tracks";

    private static string FormatPlayCount(int count) => count == 1 ? "1 play" : $"{count} plays";
}
