using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using LibVLCSharp.Shared;
using music_library.Models;
using music_library.Services;


namespace music_library.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private VLCMediaEngine _mediaEngine;
    private Song _song;


    public MainViewModel(){
        _mediaEngine = new VLCMediaEngine();
        _song = new Song("Grudgefunk", "artist", 100, "/path/to/music/file", PlaybackState.Stopped);
    }

    

    [RelayCommand]
    public void PlayMusic(){
        _mediaEngine.Play(_song.getFilePath());
    }

    [RelayCommand]
    public void TogglePause(){
        if(_mediaEngine.getPlaybackStatus()){
            _mediaEngine.Pause();
        }
        else{
            _mediaEngine.Resume();   
        }
    }

    [RelayCommand]
    public void StopMusic(){
        _mediaEngine.Stop();
    }
}
