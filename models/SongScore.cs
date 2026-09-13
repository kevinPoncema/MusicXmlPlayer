using System.Collections.Generic;
using System.Linq;

namespace MusicXmlPlayer.Models;

public class SongScore
{
    public string Title { get; set; } = "Untitled";
    public string Composer { get; set; } = "Unknown";
    public string Rights { get; set; } = "None";
    public string Encoder { get; set; } = "Unknown";
    public int TempoBpm { get; set; } = 120;
    public int Divisions { get; set; } = 1;
    public List<Measure> Measures { get; set; } = [];

    public IEnumerable<IPlayable> Timeline => Measures.SelectMany(m => m.Elements);
}