using System;
using System.IO;
using System.Globalization;
using MusicXmlPlayer.Services;

Console.WriteLine("=== MusicXML Player ===");
const int filePathIndex = 0;
const int volumeIndex = 1;
string filePath = args.Length > filePathIndex ? args[filePathIndex] : "sample.musicxml";

// Parsear volumen del segundo argumento (si existe)
double volume = 0.2;
if (args.Length > volumeIndex)
{
    if (!double.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out volume))
    {
        Console.WriteLine($"Advertencia: '{args[1]}' no es un volumen válido. Usando {volume} por defecto.");
    }
}

if (!File.Exists(filePath))
{
    Console.WriteLine($"Error: No se encontró el archivo '{filePath}'.");
    Console.WriteLine("Uso: dotnet run <archivo.musicxml> [volumen(0.0 - 1.0)]");
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

var player = new MusicPlayerService();
Console.WriteLine($"Reproduciendo con volumen a {volume * 100}%...");
player.Play(timeline, volume);

Console.WriteLine("Reproducción finalizada.");
