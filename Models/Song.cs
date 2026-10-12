

namespace music_library.Models;

public class Song{
    string title {get; set; }
    string author {get; set; }
    int duration {get; set; }
    string filePath {get; set;}
    PlaybackState playbackState {get; set;}

    public Song(string title, string author, int duration, string filePath, PlaybackState playbackState){
        this.title = title;
        this.author = author;
        this.duration = duration;
        this.filePath = filePath;
        this.playbackState = playbackState;
    }

    public string getFilePath(){
        return filePath;
    }


}