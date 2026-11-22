namespace music_library.Models;
using System;
using System.Text.Json.Serialization;
using System.Threading;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(Song), "song")]
[JsonDerivedType(typeof(Recording), "recording")]
public abstract class Audio{
    private static int _nextId;

    public string Id { get; init; } = $"trk-{Interlocked.Increment(ref _nextId):D3}";
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public PlaybackState State { get; set; }
    public int PlayCount { get; set; }

    //public abstract string Kind { get; }
    //public abstract string Description { get; }

    [JsonIgnore]
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

    internal static void EnsureIdCounterAtLeast(string? id){
        if (string.IsNullOrEmpty(id) || !id.StartsWith("trk-", StringComparison.Ordinal)){
            return;
        }

        if (!int.TryParse(id.AsSpan(4), out int value)){
            return;
        }

        int current;

        do{
            current = Volatile.Read(ref _nextId);

            if (current >= value){
                return;
            }
        }
        while (Interlocked.CompareExchange(ref _nextId, value, current) != current);
    }

    public override string ToString() => $"{Author} - {Title}";
}