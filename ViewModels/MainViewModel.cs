using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using LibVLCSharp.Shared;
using music_library.Models;
using music_library.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Threading.Tasks;

namespace music_library.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    //fields
    private VLCMediaEngine _mediaEngine;


    //observable proerties
    public Playlist Playlist { get; } = new();

    [ObservableProperty]
    private bool _isPaneOpen = false;

    [ObservableProperty]
    private Song? _selectedSong;


    //init
    public MainViewModel(){
        _mediaEngine = new VLCMediaEngine();

        
        //PlaylistSongs.Add("Feel It All Again");
        //PlaylistSongs.Add("GRUDGEFUNK!!");

        
    }

    //hooks
    partial void OnSelectedSongChanged(Song? value)
    {
        PlaySelected();
    }


    //relay commands
    [RelayCommand]
    public void PlaySelected(){
        if(SelectedSong is null){
            return;
        }
        _mediaEngine.Play(SelectedSong.getFilePath());
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

    [RelayCommand]
    public void PlayNext(){
        if (Playlist.Count == 0){
            return;
        }

        int currentIndex = Playlist.IndexOf(SelectedSong);
        int nextIndex = currentIndex + 1;

        if(nextIndex >= Playlist.Count){
            nextIndex = 0;
        }
        
        SelectedSong = Playlist[nextIndex];
    }

    [RelayCommand]
    public void PlayPrevious(){
        if (Playlist.Count == 0){
            return;
        }

        int currentIndex = Playlist.IndexOf(SelectedSong);
        int nextIndex = currentIndex - 1;

        if(nextIndex < 0){
            nextIndex = Playlist.Count - 1;
        }
        
        SelectedSong = Playlist[nextIndex];
    }

    [RelayCommand]
    public void TogglePane(){
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    private async Task SelectFolder(TopLevel? topLevel)
    {
        if (topLevel == null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Music Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            var folder = folders[0];
            var folderPath = folder.Path.LocalPath;
            Console.WriteLine(folderPath);

            await foreach (var item in folder.GetItemsAsync())
                {
                   
                    if (item is IStorageFile file)
                    {
                        
                        string? filePath = file.Path.LocalPath;
                        
                        Playlist.AddSongFromPath(filePath);
                        
                    }
                }
            
        }
    }


    

    //methods

}
