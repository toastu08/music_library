namespace music_library.Models;

using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public class Playlist : ObservableCollection<Song>{


    public void AddSongFromPath(string? path){
        if (path is null){
            Console.WriteLine("error throw exception (fix later)");

            return;
        }

        string fileName = path.Split('/')[^1];

        if (fileName.Contains(" - ")){
            string author = fileName.Split('-')[0].Trim();
            string title = fileName.Split('-')[^1].Trim();
            Song song = new Song(title, author, 100, path, PlaybackState.Stopped);
            Add(song);
        }
        else{
            string title = fileName.Trim();
            string author = "Unknown";
            Song song = new Song(title, author, 100, path, PlaybackState.Stopped);
            Add(song);
        }
        

    }

    public Song Get(int index) => this[index];
}