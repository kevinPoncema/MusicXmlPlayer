using System;
using System.IO;
using System.Linq;
using System.Globalization;
using MusicXmlPlayer.Services;

Console.WriteLine("=== MusicXML Player & Engraver ===");

const int FilePathArgIndex = 0;
const int VolumeArgIndex = 1;

bool generateImage = args.Contains("--image");
var cleanArgs = args.Where(a => a != "--image").ToArray();

string filePath = cleanArgs.Length > FilePathArgIndex ? cleanArgs[FilePathArgIndex] : "sample.musicxml";

double volume = 0.2;
if (cleanArgs.Length > VolumeArgIndex)
{
    if (!double.TryParse(cleanArgs[VolumeArgIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out volume))
    {
        Console.WriteLine($"Advertencia: '{cleanArgs[VolumeArgIndex]}' no es un volumen válido. Usando {volume} por defecto.");
    }
}

if (!File.Exists(filePath))
{
    Console.WriteLine($"Error: No se encontró el archivo '{filePath}'.");
    Console.WriteLine("Uso: dotnet run <archivo.musicxml> [volumen(0.0 - 1.0)] [--image]");
    return;
}

Console.WriteLine($"Parseando archivo: {filePath}");

var parser = new MusicXmlParser();
var score = parser.ParseScore(filePath);
var timeline = score.Timeline.ToList();

Console.WriteLine("\n--- Metadatos de la Partitura ---");
Console.WriteLine($"Título    : {score.Title}");
Console.WriteLine($"Compositor: {score.Composer}");
Console.WriteLine($"Derechos  : {score.Rights}");
Console.WriteLine($"Software  : {score.Encoder}");
Console.WriteLine($"Tempo     : {score.TempoBpm} BPM");
Console.WriteLine("---------------------------------\n");

Console.WriteLine($"Se extrajeron {timeline.Count} elementos (notas/silencios).");

if (generateImage)
{
    var engraver = new MusicEngraverService();
    string outputImg = Path.ChangeExtension(filePath, ".png");
    engraver.GenerateImage(filePath, outputImg);
}

var player = new MusicPlayerService();
Console.WriteLine($"Reproduciendo con volumen a {volume * 100}%...");
player.Play(timeline, volume);

Console.WriteLine("Reproducción finalizada.");
