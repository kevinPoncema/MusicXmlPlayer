using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MusicXmlPlayer.Services;

public class MusicEngraverService
{
    public void GenerateImage(string inputXmlPath, string outputImagePath)
    {
        Console.WriteLine($"[MusicEngraver] Generando imagen en {outputImagePath} usando MuseScore...");
        
        try
        {
            string fileName = "mscore";
            string arguments = $"-o \"{outputImagePath}\" \"{inputXmlPath}\"";

            var psi = new ProcessStartInfo
            {
                FileName = fileName, 
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process != null)
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Console.WriteLine("[MusicEngraver] ¡Partitura generada exitosamente!");
                }
                else
                {
                    string error = process.StandardError.ReadToEnd();
                    Console.WriteLine($"[MusicEngraver] Error de MuseScore (Code {process.ExitCode}): {error}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MusicEngraver] Error al intentar ejecutar MuseScore: {ex.Message}");
            Console.WriteLine("[MusicEngraver] Asegúrate de tener MuseScore instalado (apt install musescore3 xvfb).");
        }
    }
}
