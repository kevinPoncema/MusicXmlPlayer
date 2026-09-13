# Fase 1: Build y Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el proyecto y compilar
COPY . .
RUN dotnet publish "MusicXmlPlayer.csproj" -c Release -o /app/publish

# Fase 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

# Instalar dependencias esenciales:
# 1. alsa-utils: para tener 'aplay' y poder emitir sonido
# 2. musescore3 y xvfb: para renderizar partituras en un contenedor sin interfaz gráfica
RUN apt-get update && apt-get install -y \
    alsa-utils \
    musescore3 \
    xvfb \
    && rm -rf /var/lib/apt/lists/*

# Crear un alias simbólico por si el ejecutable de musescore3 no está como 'mscore'
RUN ln -sf /usr/bin/musescore3 /usr/bin/mscore || true

# Copiar los binarios de la aplicación compilada
COPY --from=build /app/publish .

# Copiar archivo de muestra
COPY sample.musicxml .

# NOTA PARA EL SONIDO:
# Para escuchar sonido desde Docker, debes pasar el dispositivo de audio usando:
# docker run --device /dev/snd mi_contenedor

ENTRYPOINT ["dotnet", "MusicXmlPlayer.dll"]
