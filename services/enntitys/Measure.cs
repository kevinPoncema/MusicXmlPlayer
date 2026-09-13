using System.Collections.Generic;

namespace MusicXmlPlayer.Models;

public class Measure
{
    public int Number { get; set; }
    public List<IPlayable> Elements { get; set; } = [];
}