namespace MusicXmlPlayer.Models;

public interface IPlayable
{
    int DurationMs { get; set; }
    bool IsRest { get; }
}