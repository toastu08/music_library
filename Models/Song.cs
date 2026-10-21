

namespace music_library.Models;

public class Song : IMedia{
    string? title {get; set; }
    string? author {get; set; }
    int duration {get; set; }
    string? filePath {get; set;}
    PlaybackState? playbackState {get; set;}

    public Song(string title, string author, int duration, string filePath, PlaybackState playbackState){
        this.title = title;
        this.author = author;
        this.duration = duration;
        this.filePath = filePath;
        this.playbackState = playbackState;
    }

    public Song(Song s){
        this.title = s.title;
        this.author = s.author;
        this.duration = s.duration;
        this.filePath = s.filePath;
        this.playbackState = s.playbackState;
    }

    public string getFilePath(){
        return filePath;
    }

    public override string ToString(){
        return author + " - " + title;
    }


}