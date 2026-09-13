using System;
using System.Collections.Generic;

namespace MusicXmlPlayer.Models;

public class NoteItem : IPlayable
{
    public string Step { get; set; } = string.Empty; // Ej: "C", "D", "F#"
    public int Octave { get; set; }                  // Ej: 4
    public int DurationMs { get; set; }              // Duración en milisegundos
    public bool IsRest => false;

    public double Frequency => CalculateFrequency();

    private double CalculateFrequency()
    {
        var semitoneOffsets = new Dictionary<string, int>
        {
            { "C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 },
            { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "G", 7 },
            { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11 }
        };

        if (!semitoneOffsets.TryGetValue(Step, out int offset))
            offset = 0;

        // Número MIDI de la nota: C4 = 60, A4 = 69
        int midiNote = 12 * (Octave + 1) + offset;
        return 440.0 * Math.Pow(2.0, (midiNote - 69.0) / 12.0);
    }
}