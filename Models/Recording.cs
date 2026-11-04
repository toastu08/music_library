namespace music_library.Models;
using System;

public class Recording : Audio{
    public Recording(string title, string author, int duration, string filePath, PlaybackState state){
        this.Title = title;
        this.Author = author;
        this.Duration = TimeSpan.FromSeconds(duration);
        this.FilePath = filePath;
        this.State = state;
    }

    public Recording(Recording r){
        this.Title = r.Title;
        this.Author = r.Author;
        this.Duration = r.Duration;
        this.FilePath = r.FilePath;
        this.State = r.State;
    }

    public override string Describe() => $"this is a recording, {LengthLabel}";
    
}