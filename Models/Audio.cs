namespace music_library.Models;
using System;
using System.Threading;

public abstract class Audio{
    private static int _nextId;

    public string Id { get; init; } = $"trk-{Interlocked.Increment(ref _nextId):D3}";
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public PlaybackState State { get; set; }

    //public abstract string Kind { get; }
    //public abstract string Description { get; }

    public string LengthLabel => Duration switch{
        var d when d >= TimeSpan.FromMinutes(20) => "a marathon",
        var d when d >= TimeSpan.FromMinutes(8) => "an epic",
        var d when d >= TimeSpan.FromMinutes(3) => "a standard track",
        _ => "a short one"
    };

    public abstract string Describe();

    public string getFilePath(){
        return FilePath;
    }

    public override string ToString() => $"{Author} - {Title}";
}