namespace music_library.Services;

using System;
using LibVLCSharp.Shared;

public class VLCMediaEngine : IMediaEngine{
    
    private LibVLC _libVLC;
    private MediaPlayer _mediaPlayer; 

    public VLCMediaEngine(){
        Core.Initialize();

        _libVLC = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVLC);
    }

    public void Play(string filePath){
        //cleanup remaining loaded media
        _mediaPlayer.Media?.Dispose();

        using var media = new Media(_libVLC, new Uri(filePath));
        _mediaPlayer.Play(media);
    }

    public void Pause(){
        _mediaPlayer.Pause();
    }

    public void Resume(){
        _mediaPlayer.Play();
    }

    public void Stop(){
        _mediaPlayer.Stop();
        _mediaPlayer.Media?.Dispose();
    }

    public bool getPlaybackStatus(){
        return _mediaPlayer.IsPlaying;
    }
}