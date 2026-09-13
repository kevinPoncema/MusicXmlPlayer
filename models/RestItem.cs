namespace MusicXmlPlayer.Models;

public class RestItem : IPlayable
{
    public int DurationMs { get; set; }
    public bool IsRest => true;
}