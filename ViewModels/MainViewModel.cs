using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Text.Json;
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
    //[ObservableProperty]
    public Library SongLibrary { get; } = new();

    public RecentlyPlayed<Audio> RecentlyPlayed { get; } = new();

    public ActivityLog Log { get; } = new();

    [ObservableProperty]
    private bool _isPaneOpen = false;

    [ObservableProperty]
    private Audio? _selectedAudio;

    [ObservableProperty]
    private int _trackCount;

    [ObservableProperty]
    private bool _isQueryResultOpen;

    [ObservableProperty]
    private string _queryResultTitle = string.Empty;

    [ObservableProperty]
    private string _queryResultBody = string.Empty;


    //init
    public MainViewModel(){
        _mediaEngine = new VLCMediaEngine();

        
        SongLibrary.AudioAdded += Log.OnAudioAdded;
        SongLibrary.AudioAdded += OnAudioAdded;

        
        SongLibrary.CollectionChanged += (_, _) => TrackCount = SongLibrary.Count;

        

        
    }

    //hooks
    partial void OnSelectedAudioChanged(Audio? value)
    {
        PlaySelected();
    }

    private void OnAudioAdded(object? sender, AudioAddedEventArgs e){
        TrackCount = e.Count;
    }


    //relay commands
    [RelayCommand]
    public void PlaySelected(){
        if(SelectedAudio is null){
            return;
        }
        RecentlyPlayed.Record(SelectedAudio);
        _mediaEngine.Play(SelectedAudio.getFilePath());
        SelectedAudio.PlayCount++;
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
        if (SongLibrary.Count == 0){
            return;
        }

        int currentIndex = SelectedAudio is null ? -1 : SongLibrary.IndexOf(SelectedAudio);
        int nextIndex = currentIndex + 1;

        if(nextIndex >= SongLibrary.Count){
            nextIndex = 0;
        }
        
        SelectedAudio = SongLibrary[nextIndex];
    }

    [RelayCommand]
    public void PlayPrevious(){
        if (SongLibrary.Count == 0){
            return;
        }

        int currentIndex = SelectedAudio is null ? -1 : SongLibrary.IndexOf(SelectedAudio);
        int nextIndex = currentIndex - 1;

        if(nextIndex < 0){
            nextIndex = SongLibrary.Count - 1;
        }
        
        SelectedAudio = SongLibrary[nextIndex];
    }

    [RelayCommand]
    public void TogglePane(){
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    private async Task SaveLibrary(){
        try{
            await LibraryStorage.SaveAsync(SongLibrary, LibraryStorage.DefaultPath);
            Log.Write($"Saved {SongLibrary.Count} tracks to {LibraryStorage.DefaultPath}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException){
            Log.Write($"Could not save the library: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task LoadLibrary(){
        try{
            var loaded = await LibraryStorage.LoadAsync(LibraryStorage.DefaultPath);

            if (loaded is null){
                Log.Write($"No saved library found at {LibraryStorage.DefaultPath}");
                return;
            }

            SongLibrary.Clear();

            foreach (Audio audio in loaded){
                SongLibrary.Add(audio);
            }

            Log.Write($"Loaded {loaded.Count} tracks from {LibraryStorage.DefaultPath}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException){
            Log.Write($"Could not load the library: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ShowMostPlayedArtist(){
        ShowQueryResult(LibraryQueries.MostPlayedArtist(SongLibrary));
    }

    [RelayCommand]
    private void ShowHoursPerGenre(){
        ShowQueryResult(LibraryQueries.HoursPerGenre(SongLibrary));
    }

    [RelayCommand]
    private void ShowNeverPlayed(){
        ShowQueryResult(LibraryQueries.NeverPlayed(SongLibrary));
    }

    [RelayCommand]
    private void ShowLongestAlbum(){
        ShowQueryResult(LibraryQueries.LongestAlbum(SongLibrary));
    }

    [RelayCommand]
    private void ShowTopDecade(){
        ShowQueryResult(LibraryQueries.TopDecade(SongLibrary));
    }

    [RelayCommand]
    private void CloseQueryResult(){
        IsQueryResultOpen = false;
    }

    private void ShowQueryResult(QueryResult result){
        QueryResultTitle = result.Title;
        QueryResultBody = result.Body;
        IsQueryResultOpen = true;
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
                        
                        SongLibrary.AddSongFromPath(filePath);
                        
                    }
                }
            
        }
    }

    [RelayCommand]
    public void DescribeAll(){
        if(SongLibrary.Count == 0){
            return;
        }

        //adding some random "recording" metadata because i dont have any
        Recording recording1 = new Recording("recording1", "author1", 198, "~recordings/recording1.mp3", PlaybackState.Stopped);
        Recording recording2 = new Recording("recording2", "author2", 287, "~recordings/recording2.mp3", PlaybackState.Stopped);
        Recording recording3 = new Recording("recording3", "author3", 900, "~recordings/recording3.mp3", PlaybackState.Stopped);
        Recording recording4 = new Recording("recording4", "author4", 3, "~recordings/recording4.mp3", PlaybackState.Stopped);
        
        SongLibrary.Add(recording1);
        SongLibrary.Add(recording2);
        SongLibrary.Add(recording3);
        SongLibrary.Add(recording4);

        SongLibrary.DescribeAll();

        //removing them because they were just metadata for testing
        SongLibrary.Remove(recording1);
        SongLibrary.Remove(recording2);
        SongLibrary.Remove(recording3);
        SongLibrary.Remove(recording4);


    }


    

    //methods

}
