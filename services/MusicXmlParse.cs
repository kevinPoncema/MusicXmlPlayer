using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MusicXmlPlayer.Models;

namespace MusicXmlPlayer.Services;

public class MusicXmlParser
{
    public List<IPlayable> ParseToTimeline(string filePath)
    {
        var score = ParseScore(filePath);
        return score.Timeline.ToList();
    }

    public SongScore ParseScore(string filePath)
    {
        var doc = XDocument.Load(filePath);
        var score = new SongScore();
        int divisions = 1;
        int tempoBpm = 120; 
        var attributesNode = doc.Descendants("attributes").FirstOrDefault();
        if (attributesNode != null)
        {
            var divNode = attributesNode.Element("divisions");
            if (divNode != null && int.TryParse(divNode.Value, out int div))
            {
                divisions = div;
                score.Divisions = div;
            }
        }

        var soundNode = doc.Descendants("sound").FirstOrDefault(s => s.Attribute("tempo") != null);
        if (soundNode != null && int.TryParse(soundNode.Attribute("tempo")?.Value, out int tempo))
        {
            tempoBpm = tempo;
            score.TempoBpm = tempo;
        }

        var titleNode = doc.Descendants("work-title").FirstOrDefault() ?? doc.Descendants("movement-title").FirstOrDefault();
        if (titleNode != null)
        {
            score.Title = titleNode.Value;
        }

        var creatorNodes = doc.Descendants("creator");
        var composerNode = creatorNodes.FirstOrDefault(c => c.Attribute("type")?.Value == "composer") ?? creatorNodes.FirstOrDefault();
        if (composerNode != null)
        {
            score.Composer = composerNode.Value;
        }

        var rightsNode = doc.Descendants("rights").FirstOrDefault();
        if (rightsNode != null)
        {
            score.Rights = rightsNode.Value;
        }

        var softwareNode = doc.Descendants("software").FirstOrDefault();
        if (softwareNode != null)
        {
            score.Encoder = softwareNode.Value;
        }

        var part = doc.Descendants("part").FirstOrDefault();
        if (part == null) return score;

        foreach (var measureNode in part.Elements("measure"))
        {
            var measure = ParseMeasure(measureNode, divisions, tempoBpm);
            score.Measures.Add(measure);
        }

        return score;
    }

    private Measure ParseMeasure(XElement measureNode, int divisions, int tempoBpm)
    {
        var measure = new Measure();
        
        if (int.TryParse(measureNode.Attribute("number")?.Value, out int num))
        {
            measure.Number = num;
        }

        foreach (var node in measureNode.Elements())
        {
            if (node.Name == "note")
            {
                var playable = ParseNoteOrRest(node, divisions, tempoBpm);
                if (playable != null)
                {
                    measure.Elements.Add(playable);
                }
            }
        }

        return measure;
    }

    private IPlayable? ParseNoteOrRest(XElement noteNode, int divisions, int tempoBpm)
    {
        var durationNode = noteNode.Element("duration");
        if (durationNode == null || !int.TryParse(durationNode.Value, out int durationTicks))
            return null; 

        double msPerTick = 60000.0 / (tempoBpm * divisions);
        int durationMs = (int)Math.Round(durationTicks * msPerTick);

        if (noteNode.Element("rest") != null)
        {
            return new RestItem
            {
                DurationMs = durationMs
            };
        }
        
        if (noteNode.Element("pitch") != null)
        {
            var pitchNode = noteNode.Element("pitch")!;
            string step = pitchNode.Element("step")?.Value ?? "C";
            
            var alterNode = pitchNode.Element("alter");
            if (alterNode != null && int.TryParse(alterNode.Value, out int alter))
            {
                if (alter == 1) step += "#";
                else if (alter == -1) step += "b"; 
            }

            step = NormalizePitch(step);

            int octave = 4;
            var octaveNode = pitchNode.Element("octave");
            if (octaveNode != null && int.TryParse(octaveNode.Value, out int oct))
            {
                octave = oct;
            }

            return new NoteItem
            {
                Step = step,
                Octave = octave,
                DurationMs = durationMs
            };
        }

        return null;
    }

    private string NormalizePitch(string step)
    {
        return step switch
        {
            "Cb" => "B",
            "Db" => "C#",
            "Eb" => "D#",
            "Fb" => "E",
            "Gb" => "F#",
            "Ab" => "G#",
            "Bb" => "A#",
            "B#" => "C",
            "E#" => "F",
            _ => step
        };
    }
}
