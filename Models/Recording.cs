namespace music_library.Models;
using System;

public class Recording : Audio{

    public Recording(){
        this.State = PlaybackState.Stopped;
    }

    public Recording(string title, string author, int duration, string filePath, PlaybackState state)
        : this(title, author, duration, filePath, state, Genre.Other, 0, "Unknown Album")
    {
    }

    public Recording(string title, string author, int duration, string filePath, PlaybackState state, Genre genre, int year, string album){
        this.Title = title;
        this.Author = author;
        this.Album = album;
        this.Genre = genre;
        this.Year = year;
        this.Duration = TimeSpan.FromSeconds(duration);
        this.FilePath = filePath;
        this.State = state;
    }

    public Recording(Recording r){
        this.Title = r.Title;
        this.Author = r.Author;
        this.Album = r.Album;
        this.Genre = r.Genre;
        this.Year = r.Year;
        this.Duration = r.Duration;
        this.FilePath = r.FilePath;
        this.State = r.State;
        this.PlayCount = r.PlayCount;
    }

    public override string Describe() => $"this is a recording, {LengthLabel}";
    
}