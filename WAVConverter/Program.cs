// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Xml.Linq;

namespace WAVConverter
{
    internal class Program
    {
        const int fileSize = 0x3F20; // 16K - 224 (size of gamebar asset)

        static void Main(string[] args)
        {
            // extract and encode the PCM data from the following WAV files
            string samplesFolder = GetSolutionFolder() + "\\game\\assets\\samples\\";
            byte[][] pcmDataSamples = [
                ExtractAndEncodePCMData(samplesFolder + "exterminate.wav"),
                ExtractAndEncodePCMData(samplesFolder + "groan.wav"),
                ExtractAndEncodePCMData(samplesFolder + "weapon.wav")];

            // stack the encoded data at page boundaries
            byte[] pcmData = new byte[fileSize];
            int[] startIndices = new int[3];
            int[] endIndices = new int[3];

            int offset = 0;
            for (int i = 0; i < pcmDataSamples.Length; i++)
            {
                int samplesLength = pcmDataSamples[i].Length;
                
                Array.Copy(pcmDataSamples[i], 0, pcmData, offset, samplesLength);

                startIndices[i] = offset + samplesLength - 1;
                endIndices[i] = offset - 1;
                
                offset += RoundToNextPage(samplesLength);
            }

            // write PCM data
            WritePCMData(pcmData, GetSolutionFolder() + "\\game\\data\\pcm.dat");

            // write address tables (copied to sound.6502)
            Console.WriteLine($".pcmDataStartAddrLoTbl EQUB &{LO(startIndices[0]):X2}, &{LO(startIndices[1]):X2}, &{LO(startIndices[2]):X2}");
            Console.WriteLine($".pcmDataStartAddrHiTbl EQUB &{HI(startIndices[0]):X2}, &{HI(startIndices[1]):X2}, &{HI(startIndices[2]):X2}");
            Console.WriteLine($".pcmDataEndAddrHiTbl   EQUB &{HI(endIndices[0]):X2}, &{HI(endIndices[1]):X2}, &{HI(endIndices[2]):X2}");
        }

        static int RoundToNextPage(int value) => (value + 255) / 256 * 256;
        static int HI(int value) => (0x8000 + value) / 256;
        static int LO(int value) => (0x8000 + value) % 256;

        static private byte[] ExtractAndEncodePCMData(string filePath)
        { 
            // read PCM data from 8/8K wav file
            byte[] pcmData = ReadPCMData(filePath);
            if (pcmData.Length == 0) return [];

            // write in low and high nibble amplitudes
            int halfLength = pcmData.Length / 2;
            for (int i = 0; i < halfLength; i++)
            {
                int firstHalfAmplitude = 0xF - (pcmData[i] >> 4);
                int secondHalfAmplitude = 0xF - (pcmData[i + halfLength] >> 4);
                pcmData[i] = (byte)(firstHalfAmplitude | (secondHalfAmplitude << 4));
            }

            // ensure last amplitude is silence
            pcmData[halfLength - 1] |= 0xF0;

            // resize array and reverse it (the runtime traverses it backwards)
            Array.Resize(ref pcmData, halfLength);
            Array.Reverse(pcmData);

            return pcmData;
        }

        static private byte[] ReadPCMData(string filePath)
        { 
            FileStream fileStream = File.OpenRead(filePath);

            // read file header
            if (fileStream.ReadByte() != 'R' ||
                fileStream.ReadByte() != 'I' ||
                fileStream.ReadByte() != 'F' ||
                fileStream.ReadByte() != 'F')
            {
                Console.WriteLine("Error, not a WAV file!");
                return [];
            }

            ReadInt(fileStream); // skip file size

            if (fileStream.ReadByte() != 'W' ||
                fileStream.ReadByte() != 'A' ||
                fileStream.ReadByte() != 'V' ||
                fileStream.ReadByte() != 'E')
            {
                Console.WriteLine("Error, not a WAV file!");
                return [];
            }

            // read format chunk
            if (fileStream.ReadByte() != 'f' ||
                fileStream.ReadByte() != 'm' ||
                fileStream.ReadByte() != 't' ||
                fileStream.ReadByte() != ' ')
            {
                Console.WriteLine("Error, WAV format chunk missing!");
                return [];
            }

            if (ReadInt(fileStream) != 16 ||
                ReadShort(fileStream) != 1)
            {
                Console.WriteLine("Error, data not PCM format!");
                return [];
            }

            if (ReadShort(fileStream) != 1 ||   // nChannels
                ReadInt(fileStream) != 8000 ||  // nSamplesPerSec
                ReadInt(fileStream) != 8000 ||  // nAvgBytesPerSec
                ReadShort(fileStream) != 1 ||   // nBlockAlign
                ReadShort(fileStream) != 8)     // wBitsPerSample
            {
                Console.WriteLine("Error, 8-bits/8KHz mono format expected!");
                return [];
            }

            // read data chunk
            if (fileStream.ReadByte() != 'd' ||
                fileStream.ReadByte() != 'a' ||
                fileStream.ReadByte() != 't' ||
                fileStream.ReadByte() != 'a')
            {
                Console.WriteLine("Error, WAV data chunk missing!");
                return [];
            }

            // read PCM data
            int pcmDataSize = ReadInt(fileStream);
            byte[] pcmData = new byte[pcmDataSize];
            fileStream.ReadExactly(pcmData);

            return pcmData;
        }

        static private void WritePCMData(byte[] pcmData, string filePath)
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
            using (FileStream fileStream = File.OpenWrite(filePath))
            {
                // write data
                fileStream.Write(pcmData);
            }
        }

        private static short ReadShort(FileStream fileStream)
        {
            return (short)(fileStream.ReadByte() | (fileStream.ReadByte() << 8));
        }

        private static int ReadInt(FileStream fileStream)
        {
            return fileStream.ReadByte() | (fileStream.ReadByte() << 8) | (fileStream.ReadByte() << 16) | (fileStream.ReadByte() << 24);
        }

        static private string GetSolutionFolder()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && directory.GetFiles("*.sln").Length == 0)
            {
                directory = directory.Parent;
            }
            return (directory != null) ? directory.FullName : string.Empty;
        }
    }
}
