using music_library.Models;
using music_library.Services;

namespace music_library.Tests;

public class LibraryStorageTests
{
    private static string TempPath() =>
        Path.Combine(Path.GetTempPath(), $"music_library_{Guid.NewGuid():N}.json");

    [Fact]
    public async Task SaveAndLoad_RestoresEachKindWithAllProperties()
    {
        string path = TempPath();

        try
        {
            var library = new Library();
            var song = new Song("Song Title", "Song Author", 238, "/music/song.mp3", PlaybackState.Paused)
            {
                Id = "trk-001",
                PlayCount = 7
            };
            var recording = new Recording("Recording Title", "Recording Author", 900, "/music/recording.mp3", PlaybackState.Stopped)
            {
                Id = "trk-002",
                PlayCount = 3
            };
            library.Add(song);
            library.Add(recording);

            await LibraryStorage.SaveAsync(library, path);
            var loaded = await LibraryStorage.LoadAsync(path);

            Assert.NotNull(loaded);
            var tracks = loaded!;
            Assert.Equal(2, tracks.Count);

            var loadedSong = Assert.IsType<Song>(tracks[0]);
            Assert.Equal("trk-001", loadedSong.Id);
            Assert.Equal("Song Title", loadedSong.Title);
            Assert.Equal("Song Author", loadedSong.Author);
            Assert.Equal(TimeSpan.FromSeconds(238), loadedSong.Duration);
            Assert.Equal("/music/song.mp3", loadedSong.FilePath);
            Assert.Equal(PlaybackState.Paused, loadedSong.State);
            Assert.Equal(7, loadedSong.PlayCount);

            var loadedRecording = Assert.IsType<Recording>(tracks[1]);
            Assert.Equal("trk-002", loadedRecording.Id);
            Assert.Equal("Recording Title", loadedRecording.Title);
            Assert.Equal("Recording Author", loadedRecording.Author);
            Assert.Equal(TimeSpan.FromSeconds(900), loadedRecording.Duration);
            Assert.Equal("/music/recording.mp3", loadedRecording.FilePath);
            Assert.Equal(PlaybackState.Stopped, loadedRecording.State);
            Assert.Equal(3, loadedRecording.PlayCount);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [Fact]
    public async Task LoadAsync_ReturnsNullWhenFileIsMissing()
    {
        string path = TempPath();

        var loaded = await LibraryStorage.LoadAsync(path);

        Assert.Null(loaded);
    }

    [Fact]
    public async Task LoadAsync_AdvancesTheIdCounter()
    {
        string path = TempPath();

        try
        {
            var library = new Library();
            var song = new Song("Title", "Author", 100, "/music/song.mp3", PlaybackState.Stopped)
            {
                Id = "trk-900000"
            };
            library.Add(song);

            await LibraryStorage.SaveAsync(library, path);
            await LibraryStorage.LoadAsync(path);

            var next = new Song("Next", "Author", 100, "/music/next.mp3", PlaybackState.Stopped);
            int nextId = int.Parse(next.Id.AsSpan(4));

            Assert.True(nextId > 900000);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [Fact]
    public async Task SaveAsync_WritesPolymorphicJsonWithoutComputedProperties()
    {
        string path = TempPath();

        try
        {
            var library = new Library();
            library.Add(new Song("Title", "Author", 100, "/music/song.mp3", PlaybackState.Stopped));

            await LibraryStorage.SaveAsync(library, path);

            string json = await File.ReadAllTextAsync(path);

            Assert.Contains("\"kind\": \"song\"", json);
            Assert.DoesNotContain("lengthLabel", json, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
