# MusicXML Player

Un reproductor por consola desarrollado en **C# (.NET)** diseñado para analizar y reproducir archivos en formato estándar **MusicXML**. Este proyecto fue creado con una filosofía de compatibilidad pura en entornos Linux (específicamente Ubuntu), utilizando la librería **NAudio** para sintetizar audio y `aplay` para reproducirlo sin dependencias gráficas de Windows.

## Características Principales

- 🎼 **Parseador Nativo:** Lee etiquetas estándar de archivos `.musicxml` (Notas, Silencios, Compases, BPM).
- 🏷️ **Metadatos:** Extrae e imprime en pantalla el Título, Compositor, Derechos de Autor y el Software utilizado para crear la partitura.
- 🔊 **Síntesis de Audio (Cross-Platform):** Genera dinámicamente frecuencias sinusoidales precisas según las notas del MusicXML utilizando los generadores de `NAudio`.
- 🐧 **Soporte Nativo Linux:** Evita el uso de APIs privativas de Windows (como `WaveOut`), utilizando en su lugar un motor de renderizado a `.wav` para delegar la salida de sonido directamente a `aplay` (ALSA).
- 🎚️ **Control de Volumen Dinámico:** Puedes controlar la amplificación (ganancia) directamente desde el terminal.

## Requisitos Previos

- [.NET SDK](https://dotnet.microsoft.com/download) (versión 10.0 o superior recomendada).
- SO Linux (probado en Ubuntu) con el comando `aplay` disponible (viene por defecto con ALSA).

## Uso y Ejecución

Puedes reproducir cualquier archivo `.musicxml`. Si no pasas argumentos, el programa buscará por defecto un archivo llamado `sample.musicxml`.

```bash
# Reproducir el archivo de muestra (Estrellita Dónde Estás)
dotnet run

# Reproducir un archivo específico
dotnet run tu_archivo.musicxml

# Reproducir un archivo específico con volumen ajustable (0.0 a 1.0)
dotnet run tu_archivo.musicxml 0.5
```

## Estructura del Proyecto

- `Models/`: Clases de dominio como `SongScore`, `Measure`, `NoteItem` y `RestItem`.
- `Services/`:
  - `MusicXmlParser`: Analiza el DOM del XML para convertirlo en objetos tipados.
  - `MusicPlayerService`: Procesa la lista de `IPlayable`, genera los osciladores y acciona el reproductor.
- `Program.cs`: Punto de entrada del programa, inyector de dependencias rudimentario y capa visual de la CLI.

## Autor

Proyecto personal desarrollado en C#.
