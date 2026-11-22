namespace music_library.Models;
using System;

public class Song : Audio{

    public Song(){
        this.State = PlaybackState.Stopped;
    }
    
    public Song(string title, string author, int duration, string filePath, PlaybackState state)
        : this(title, author, duration, filePath, state, Genre.Other, 0, "Unknown Album")
    {
    }

    public Song(string title, string author, int duration, string filePath, PlaybackState state, Genre genre, int year, string album){
        this.Title = title;
        this.Author = author;
        this.Album = album;
        this.Genre = genre;
        this.Year = year;
        this.Duration = TimeSpan.FromSeconds(duration);
        this.FilePath = filePath;
        this.State = state;
    }

    public Song(Song s){
        this.Title = s.Title;
        this.Author = s.Author;
        this.Album = s.Album;
        this.Genre = s.Genre;
        this.Year = s.Year;
        this.Duration = s.Duration;
        this.FilePath = s.FilePath;
        this.State = s.State;
        this.PlayCount = s.PlayCount;
    }

    

    public override string Describe() => $"this is a song, {LengthLabel}";
    


}
