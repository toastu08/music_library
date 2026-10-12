namespace music_library.Services;


public interface IMediaEngine{
    public void Play(string filePath);
    public void Pause();
    public void Resume();
    public void Stop();
    public bool getPlaybackStatus();
    
}