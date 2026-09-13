# MusicXML Player & Engraver

Una herramienta por consola desarrollada en **C# (.NET)** diseñada para analizar, reproducir y renderizar gráficamente archivos en formato estándar **MusicXML**. Este proyecto está construido con una filosofía de compatibilidad pura, soportando entornos Linux (como Ubuntu) nativamente e integrado opcionalmente en contenedores Docker. 

## Características Principales

- 🎼 **Parseador Nativo:** Lee etiquetas estándar de archivos `.musicxml` (Notas, Silencios, Compases, BPM).
- 🏷️ **Metadatos:** Extrae e imprime en pantalla el Título, Compositor, Derechos de Autor y el Software original de la partitura.
- 🔊 **Síntesis de Audio (Cross-Platform):** Genera frecuencias precisas vía `NAudio` (SignalGenerators) y reproduce el audio usando `aplay` (ALSA) para evadir las dependencias exclusivas de Windows como `WaveOut`.
- 🎚️ **Control de Volumen Dinámico:** Puedes configurar la amplificación (0.0 a 1.0) desde la línea de comandos.
- 🎨 **Generador Gráfico de Partituras:** Utilizando el flag `--image`, dibuja visualmente un archivo `.png` perfecto de tu XML, integrándose *headless* con la interfaz de consola de **MuseScore** (`mscore` / `xvfb-run`).
- 🐳 **Dockerización Universal:** Listo para ejecutarse de principio a fin en cualquier sistema vía Docker, inyectando el hardware de sonido local.

## Requisitos (Uso Local)

Si prefieres correrlo en tu máquina host sin Docker:
- [.NET SDK](https://dotnet.microsoft.com/download) (versión 10.0+).
- SO Linux con `aplay` (ALSA) instalado.
- Dependencias opcionales para usar `--image`: `sudo apt install musescore3 xvfb`

## Uso y Ejecución

Si no pasas un archivo, por defecto buscará y ejecutará `sample.musicxml`. Los parámetros admitidos incluyen un archivo, un volumen decimal y el flag `--image` en cualquier orden válido.

```bash
# 1. Reproducción básica
dotnet run

# 2. Reproducir un archivo específico
dotnet run tu_archivo.musicxml

# 3. Reproducir con ajuste de volumen (ej: 80%)
dotnet run tu_archivo.musicxml 0.8

# 4. Reproducir con volumen Y generar partitura en formato PNG
dotnet run tu_archivo.musicxml 0.8 --image
```

## Ejecución Mediante Docker

Para evitar ensuciar tu sistema operativo instalando MuseScore o .NET, puedes delegarle todo el trabajo al contenedor.

1. **Construir la imagen** (sólo la primera vez):
   ```bash
   docker build -t music-xml-player .
   ```

2. **Correr la aplicación** (con sonido e imágenes):
   > ⚠️ **Nota:** Para que el contenedor pueda reproducir sonido en tus audífonos/parlantes físicos, es estrictamente necesario mapear la tarjeta de sonido de tu sistema host hacia dentro de Docker utilizando el parámetro `--device /dev/snd`.

   ```bash
   docker run --device /dev/snd music-xml-player sample.musicxml 0.8 --image
   ```

## Estructura del Proyecto

- `Models/`: Modelos del dominio (`SongScore`, `Measure`, `NoteItem`, `RestItem`).
- `Services/`:
  - `MusicXmlParser`: Convierte los nodos XML a objetos tipados.
  - `MusicPlayerService`: Procesa cronológicamente la música y la emite hacia ALSA.
  - `MusicEngraverService`: Administra los procesos de MuseScore para renderizar partituras visuales.
- `Program.cs`: Punto de entrada CLI, parser de argumentos y orquestador.
- `Dockerfile`: Receta multi-stage que automatiza el empaquetado de MuseScore, XVFB y ALSA.
