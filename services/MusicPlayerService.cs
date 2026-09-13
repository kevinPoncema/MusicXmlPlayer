using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using MusicXmlPlayer.Models;

namespace MusicXmlPlayer.Services;

public class MusicPlayerService
{
    public void Play(List<IPlayable> timeline, double volume = 0.2)
    {
        var sampleProviders = new List<ISampleProvider>();
        // Usamos formato Mono a 44.1kHz que es bastante universal
        var format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);

        foreach (var item in timeline)
        {
            if (item is RestItem rest)
            {
                var silence = new SilenceProvider(format)
                    .ToSampleProvider()
                    .Take(TimeSpan.FromMilliseconds(rest.DurationMs));
                sampleProviders.Add(silence);
            }
            else if (item is NoteItem note)
            {
                var signal = new SignalGenerator(44100, 1)
                {
                    Type = SignalGeneratorType.Sin,
                    Frequency = note.Frequency,
                    Gain = volume // Volumen ajustable
                }.Take(TimeSpan.FromMilliseconds(note.DurationMs));
                sampleProviders.Add(signal);
            }
        }

        if (sampleProviders.Count == 0)
        {
            Console.WriteLine("[MusicPlayer] No hay notas para reproducir.");
            return;
        }

        // Concatenar todas las notas/silencios en una sola pista
        var playlist = new ConcatenatingSampleProvider(sampleProviders);

        // Como NAudio WaveOut es exclusivo de Windows y estamos en Linux/Ubuntu,
        // la manera cross-platform es renderizar un WAV y usar aplay (ALSA).
        string tempFile = Path.Combine(Path.GetTempPath(), "musicxml_score.wav");
        WaveFileWriter.CreateWaveFile16(tempFile, playlist);

        Console.WriteLine($"[MusicPlayer] Audio generado. Reproduciendo...");

        // Llamar a aplay para reproducir en Ubuntu
        PlayWithAplay(tempFile);
    }

    private void PlayWithAplay(string filePath)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "aplay",
                Arguments = $"-q \"{filePath}\"", // -q = modo silencioso (quiet)
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MusicPlayer] Error al ejecutar 'aplay': {ex.Message}");
            Console.WriteLine($"Puedes reproducir manualmente el archivo temporal: {filePath}");
        }
    }
}
