namespace music_library.Models;

using System;

public sealed class AudioAddedEventArgs : EventArgs
{
    public AudioAddedEventArgs(Audio audio, int count)
    {
        Audio = audio;
        Count = count;
    }

    public Audio Audio { get; }

    public int Count { get; }
}
