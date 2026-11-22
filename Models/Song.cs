namespace music_library.Models;
using System;

public class Song : Audio{

    public Song(){
        this.State = PlaybackState.Stopped;
    }
    
    public Song(string title, string author, int duration, string filePath, PlaybackState state){
        this.Title = title;
        this.Author = author;
        this.Duration = TimeSpan.FromSeconds(duration);
        this.FilePath = filePath;
        this.State = state;
    }

    public Song(Song s){
        this.Title = s.Title;
        this.Author = s.Author;
        this.Duration = s.Duration;
        this.FilePath = s.FilePath;
        this.State = s.State;
        this.PlayCount = s.PlayCount;
    }

    

    public override string Describe() => $"this is a song, {LengthLabel}";
    


}
