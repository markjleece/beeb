// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Diagnostics;

namespace MIDIConverter
{

    internal class Program
    {
        static void Main(string[] args)
        {
            // generate pitch table, writing table to console (its copied & pasted into music.6502)
            GeneratePitchTable();

            // convert MIDI file
            ConvertMIDIFile("C:\\Dev\\Beeb\\music.mid", "C:\\Dev\\Beeb\\Dalek\\music.dat");
        }

        private static void GeneratePitchTable()
        {
            // calculate pitch table values, which are a quarter-semi-tone apart
            int[] values = new int[256];

            for (int i = 0; i < 256; i++)
            {
                double n = 7.0 + (double)i / 4.0;
                double frequency = Math.Pow(2.0, (double)(n - 49.0) / 12.0) * 440.0; // formula from wikipedia
                if (i < 80)
                {
                    // calculate low-frequency timer value
                    values[i] = (int)Math.Round(500000.0 / frequency, MidpointRounding.ToZero);
                }
                else
                {
                    // calculate tone generator frequency value (formlula from Beeb Advanced User Guide)
                    values[i] = (int)Math.Round(4000000.0 / (32.0 * frequency), MidpointRounding.ToZero);
                }
            }

            // write low-byte table
            Console.WriteLine($".pitchTblLo");
            WriteTableRange("timer-based frequencies...", values, 0/*from*/, 79/*to*/, (int value) => (value % 256));
            WriteTableRange("native frequencies...", values, 80/*from*/, 255/*to*/, (int value) => (value % 16));

            // write high-byte table
            Console.WriteLine($".pitchTblHi");
            WriteTableRange("timer-based frequencies...", values, 0/*from*/, 79/*to*/, (int value) => (value / 256));
            WriteTableRange("native frequencies...", values, 80/*from*/, 255/*to*/, (int value) => (value / 16));
        }

        private static void WriteTableRange(string comment, int[] values, int from, int to, Func<int, int> func)
        {
            Console.WriteLine($"    ; {comment}");
            int index = 0;
            for (int i = 0; i < 16; i++)
            {
                if (index >= from && index <= to)
                {
                    Console.Write("    EQUB ");
                }

                for (int j = 0; j < 16; j++)
                {
                    if (index >= from && index <= to)
                    {
                        byte value = (byte)func(values[index]);
                        Console.Write($"&{value:X2}");
                        if (j < 15)
                        {
                            Console.Write(", ");
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                    }

                    index++;
                }
            }
            Console.WriteLine();
        }

        private static void ConvertMIDIFile(string inputFilePath, string outputFilePath)
        {
            // extract note events from midi file
            List<NoteEvent> noteEvents = ExtractNoteEventsFromMIDIFile(inputFilePath);

            // ensure second event has a delta time (Sibelius's MIDI export gets this wrong)
            noteEvents[1].DeltaTime = 12;

            // convert all delta times to absolute times, which simplifies sorting and filtering (we'll restore these later)
            int absoluteTime = 0;
            foreach (var noteEvent in noteEvents)
            {
                absoluteTime += noteEvent.DeltaTime;
                noteEvent.DeltaTime = absoluteTime;
            }

            // sort all note events by absolute time, volume, and tone generator
            noteEvents.Sort(CompareNoteEventsByTimeVolumeAndPitch);

            // assign note events to tone generator 0..2
            int[] toneGenerators = [Unassigned, Unassigned, Unassigned];
            foreach (var noteEvent in noteEvents)
            {
                if (noteEvent.Amplitude > 0)
                {
                    // assigned note event to tone generator
                    bool assigned = false;

                    for (int i = 0; i < toneGenerators.Length; i++)
                    {
                        if (toneGenerators[i] == Unassigned)
                        {
                            // check that only line notes are assigned to tone generator zero
                            if ((noteEvent.Pitch < LowerCPitch) != (i == 0))
                            {
                                throw new Exception("base note not assigned to tone generator 0");
                            }

                            noteEvent.ToneGenerator = i;
                            toneGenerators[i] = noteEvent.Pitch;
                            assigned = true;
                            break;
                        }
                    }

                    if (!assigned)
                    {
                        throw new Exception("note event not assigned to tone generator");
                    }
                }
                else // if (noteEvent.Amplitude == 0)
                {
                    // unassign note event from tone generator
                    bool unassigned = false;

                    for (int i = 0; i < toneGenerators.Length; i++)
                    {
                        if (noteEvent.Pitch == toneGenerators[i])
                        {
                            noteEvent.ToneGenerator = i;
                            toneGenerators[i] = Unassigned;
                            unassigned = true;
                            break;
                        }
                    }

                    if (!unassigned)
                    {
                        throw new Exception("note event not unassigned from tone generator");
                    }
                }
            }

            for (int i = 0; i < toneGenerators.Length; i++)
            {
                if (toneGenerators[i] != Unassigned)
                {
                    throw new Exception("note event assigned to tone generator at end");
                }
            }

            // sort all note events by absolute time, volume, and tone generator
            noteEvents.Sort(CompareNoteEventsByTimeToneGeneratorAndVolume);

            // remove any rudundant note off events
            List<NoteEvent> eventsToRemove = new List<NoteEvent>();
            NoteEvent? lastNoteEvent = null;
            foreach (var noteEvent in noteEvents)
            {
                if (lastNoteEvent != null &&
                    noteEvent.DeltaTime == lastNoteEvent.DeltaTime &&
                    noteEvent.Amplitude != 0 && lastNoteEvent.Amplitude == 0 &&
                    noteEvent.ToneGenerator == lastNoteEvent.ToneGenerator)
                {
                    eventsToRemove.Add(lastNoteEvent);
                }

                lastNoteEvent = noteEvent;
            }

            foreach (var noteEvent in eventsToRemove)
            {
                noteEvents.Remove(noteEvent);
            }

            // convert absolute times to 1/100th second units
            foreach (var noteEvent in noteEvents)
            {
                noteEvent.DeltaTime = (int)Math.Round(0.31 * (double)noteEvent.DeltaTime, MidpointRounding.ToZero);
            }

            // restore delta times
            int lastAbsoluteTime = 0;
            foreach (var noteEvent in noteEvents)
            {
                int deltaTime = noteEvent.DeltaTime - lastAbsoluteTime;
                lastAbsoluteTime = noteEvent.DeltaTime;
                noteEvent.DeltaTime = deltaTime;
            }

            // debug print note events, with bar markers
            int barNumber = 1;
            int timeAccumulator = 0;
            DebugWriteLine($"bar: {barNumber++}");
            foreach (var noteEvent in noteEvents)
            {
                timeAccumulator += noteEvent.DeltaTime;
                if (timeAccumulator >= 96*6)
                {
                    DebugWriteLine($"bar: {barNumber++}");
                    timeAccumulator = 0;
                }

                DebugWriteLine($"deltaTime: {noteEvent.DeltaTime}, tone generator: {noteEvent.ToneGenerator}, amplitude: {noteEvent.Amplitude}, pitch# {noteEvent.Pitch}");
            }

            // write note events to binary file
            WriteNoteEvents(noteEvents, outputFilePath);
        }

        static List<NoteEvent> ExtractNoteEventsFromMIDIFile(string filePath)
        {
            List<NoteEvent> noteEvents = new List<NoteEvent>();

            FileStream fileStream = File.OpenRead(filePath);

            //
            // Read header chunk
            //
            if (ReadByte(fileStream) != 'M' ||
                ReadByte(fileStream) != 'T' ||
                ReadByte(fileStream) != 'h' ||
                ReadByte(fileStream) != 'd')
            {
                throw new Exception("MIDI header expected");
            }

            if (ReadInt32(fileStream) != 6)
            {
                throw new Exception("MIDI header not six bytes");
            }

            if (ReadInt16(fileStream) != 0)
            {
                throw new Exception("MIDI type not 0");
            }

            if (ReadInt16(fileStream) != 1)
            {
                throw new Exception("MIDI single track expected");
            }
            if (ReadInt16(fileStream) != 0x60)
            {
                throw new Exception("MIDI time signature not 96 per quarter note");
            }

            //
            // Read track chunk
            //
            if (ReadByte(fileStream) != 'M' ||
                ReadByte(fileStream) != 'T' ||
                ReadByte(fileStream) != 'r' ||
                ReadByte(fileStream) != 'k')
            {
                throw new Exception("MIDI header expected");
            }

            int trackDataSize = ReadInt32(fileStream);

            // load track chunk data
            byte[] trackData = new byte[trackDataSize];
            if (fileStream.Read(trackData, 0, trackDataSize) != trackDataSize)
            {
                throw new Exception("MIDI track chunk track read error");
            }
            int trackDataIndex = 0;

            //
            // collect note events
            //
            while (trackDataIndex < trackDataSize)
            {
                // read delta time
                int deltaTime = ReadVarLen(trackData, ref trackDataIndex);

                // read and interpret event
                byte trackEvent = trackData[trackDataIndex++];

                if (trackEvent < 0xF0)
                {
                    byte midiNoteNumber, velocity, pressure, controllerNumber, newValue, programNumber;
                    int channel = (trackEvent & 0xF);
                    switch (trackEvent & 0xF0)
                    {
                        case 0x80: // Note Off
                            midiNoteNumber = trackData[trackDataIndex++];
                            velocity = trackData[trackDataIndex++];
                            noteEvents.Add(new NoteEvent(deltaTime, 0/*volume*/, midiNoteNumber));
                            DebugWriteLine($"Note Off - note#: {midiNoteNumber}, velocity: {velocity}, delta time: {deltaTime}");
                            break;

                        case 0x90: // Note On 
                            midiNoteNumber = trackData[trackDataIndex++];
                            velocity = trackData[trackDataIndex++];
                            noteEvents.Add(new NoteEvent(deltaTime, Math.Max(velocity, (byte)1), midiNoteNumber));
                            DebugWriteLine($"Note On - note#: {midiNoteNumber}, velocity: {velocity}, delta time: {deltaTime}");
                            break;

                        case 0xA0: // Polyphonic Key Pressure
                            midiNoteNumber = trackData[trackDataIndex++];
                            pressure = trackData[trackDataIndex++];
                            DebugWriteLine($"Polyphonic Key Pressure - note#: {midiNoteNumber}, pressure: {pressure}, delta time: {deltaTime}");
                            break;

                        case 0xB0: // Control Change
                            controllerNumber = trackData[trackDataIndex++];
                            newValue = trackData[trackDataIndex++];
                            DebugWriteLine($"Control Change - controller#: {controllerNumber}, new value: {newValue}");
                            break;

                        case 0xC0: // Program Change
                            programNumber = trackData[trackDataIndex++];
                            DebugWriteLine($"Program Change - program#: {programNumber}");
                            break;

                        case 0xD0: // Channel Pressure (After-touch)
                            pressure = trackData[trackDataIndex++];
                            DebugWriteLine($"Channel Pressure (After-touch) - pressure#: {pressure}, delta time: {deltaTime}");
                            break;

                        case 0xE0: // Pitch Wheel Change
                            trackDataIndex += 2;
                            DebugWriteLine($"Pitch Wheel Change, delta time: {deltaTime}");
                            break;
                    }
                }
                else
                {
                    int length;
                    switch (trackEvent)
                    {
                        case 0xF0: // System Exclusive
                        case 0xF7:
                            length = ReadVarLen(trackData, ref trackDataIndex);
                            trackDataIndex += length;
                            DebugWriteLine("System Exclusive event");
                            break;

                        case 0xF2: // Song Position Pointer.
                            trackDataIndex += 2;
                            DebugWriteLine($"Song Position Pointer");
                            break;

                        case 0xFF: // Meta event
                            byte meteEventType = trackData[trackDataIndex++];
                            length = ReadVarLen(trackData, ref trackDataIndex);
                            trackDataIndex += length;
                            DebugWriteLine($"Meta event: 0x{meteEventType:X2}");
                            break;

                        case 0xF3: // Select Song
                            trackDataIndex++;
                            DebugWriteLine($"Select Song");
                            break;

                        case 0xF6: // Tune Request
                            DebugWriteLine($"Tune Request");
                            break;

                        case 0xF8: // Timing Clock
                            DebugWriteLine($"Timing Clock");
                            break;

                        case 0xFA: // Start
                            DebugWriteLine($"Start");
                            break;

                        case 0xFB: // Continue
                            DebugWriteLine($"Continue");
                            break;

                        case 0xFC: // Stop
                            DebugWriteLine($"Continue");
                            break;

                        case 0xFE: // Active Sensing
                            DebugWriteLine($"Active Sensing");
                            break;

                        case 0xF1:
                        case 0xF4:
                        case 0xF5:
                        case 0xF9:
                        case 0xFD:
                            DebugWriteLine($"Undefined: {trackEvent}");
                            break;
                    }
                }
            }

            return noteEvents;
        }

        static void WriteNoteEvents(List<NoteEvent> noteEvents, string filePath)
        {
            // delete existing file as File.OpenWrite(...) does will open an existing file for write
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                if (ex is not FileNotFoundException)
                {
                    throw new Exception($"Error deleting: {filePath}");
                }
            }

            // open new file for write
            using (FileStream fs = File.OpenWrite(filePath))
            {
                // write events
                foreach (NoteEvent noteEvent in noteEvents)
                {
                    // delta time
                    int deltaTime = noteEvent.DeltaTime;
                    if (deltaTime < 128)
                    {
                        fs.WriteByte((byte)deltaTime);
                    }
                    else
                    {
                        fs.WriteByte((byte)((deltaTime >> 8) | 0x80));
                        fs.WriteByte((byte)deltaTime);
                    }

                    // pitch / tone generator
                    int pitch = (noteEvent.Amplitude > 0) ? noteEvent.Pitch : 0;
                    fs.WriteByte((byte)((pitch << 2) | noteEvent.ToneGenerator));
                }

                // write terminator with one-second delay
                fs.WriteByte(0x64);
                fs.WriteByte(0xFF);
            }
        }

        static void DebugWriteLine(string value)
        {
            //Console.WriteLine("Debug: " + value);
        }

        static int ReadInt32(FileStream fileStream)
        {
            return (ReadByte(fileStream) << 24) + (ReadByte(fileStream) << 16) + (ReadByte(fileStream) << 8) + ReadByte(fileStream);
        }

        static int ReadInt16(FileStream fileStream)
        {
            return (ReadByte(fileStream) << 8) + ReadByte(fileStream);
        }

        static byte ReadByte(FileStream fileStream)
        {
            int value = fileStream.ReadByte();
            if (value == -1)
            {
                throw new Exception("ReadByte() - Unexpected error");
            }

            return (byte)value;
        }

        static int ReadVarLen(byte[] trackData, ref int trackDataIndex)
        {
            int value = trackData[trackDataIndex++];
            if ((value & 0x80) != 0)
            {
                value &= 0x7f;
                int nextByte;
                do
                {
                    nextByte = trackData[trackDataIndex++];
                    value = (value << 7) + (nextByte & 0x7f);
                } while ((nextByte & 0x80) != 0);
            }
            return value;
        }

        internal class NoteEvent
        {
            internal NoteEvent(int deltaTime, byte velocity, byte midiNoteNumber)
            {
                DeltaTime = deltaTime;
                Amplitude = (velocity > 0) ? 1 : 0;
                Pitch = (midiNoteNumber - 27);
                ToneGenerator = Unassigned;

                if (Pitch < 1 || Pitch > 63)
                {
                    throw new Exception($"Unsupported midi note number: {midiNoteNumber}");
                }
            }

            internal int DeltaTime;
            internal int Amplitude;
            internal int Pitch;
            internal int ToneGenerator;
        };

        private static int CompareNoteEventsByTimeVolumeAndPitch(NoteEvent lhs, NoteEvent rhs)
        {
            int diff = (lhs.DeltaTime - rhs.DeltaTime);
            if (diff == 0)
            {
                diff = (lhs.Amplitude - rhs.Amplitude);
                if (diff == 0)
                {
                    diff = (lhs.Pitch - rhs.Pitch);
                }
            }
            return diff;
        }

        private static int CompareNoteEventsByTimeToneGeneratorAndVolume(NoteEvent lhs, NoteEvent rhs)
        {
            int diff = (lhs.DeltaTime - rhs.DeltaTime);
            if (diff == 0)
            {
                diff = (lhs.ToneGenerator - rhs.ToneGenerator);
                if (diff == 0)
                {
                    diff = (lhs.Amplitude - rhs.Amplitude);
                }
            }
            return diff;
        }

        private const int LowerCPitch = 48-27;
        private const int Unassigned = -1;
    }
}
