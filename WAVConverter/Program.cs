// --------------------------------------------------------------
// An Adventure In Time - A Doctor Who fan game for the BBC Micro
// Model B
//
// Copyright (C) 2025  Mark John Leece
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the Free
// Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
// Boston, MA  02110-1301, USA.
// --------------------------------------------------------------

//
// This console app converts 8/8k WAV files into two packed 4/8k PCM files which
// are packaged within the game image.
// 
// The app also prints address tables which are embedded in sound.6503.  If the 
// WAV files change, the tables in sound.6502 will need updating.
//
namespace WAVConverter
{
    internal class Program
    {
        const int fileSize = 0x3F00; // 16K - 256

        static void Main(string[] _)
        {
            List<int> startIndices = [];
            List<int> endIndices = [];

            string inputFolder = GetSolutionFolder() + "\\game\\assets\\samples\\";
            string outputFolder = GetSolutionFolder() + "\\game\\data\\";

            // convert primary samples (loaded to first 16K of sideways RAM)
            byte[][] pcmDataSamples = [
                ExtractAndEncodeData(inputFolder + "dalek_exterminate.wav"),
                ExtractAndEncodeData(inputFolder + "dalek_groan.wav"),
                ExtractAndEncodeData(inputFolder + "dalek_weapon.wav")];

            WriteData(startIndices, endIndices, pcmDataSamples, outputFolder + "pcm_primary.dat");

            // convert secondary samples (loaded to second 16K of sideways RAM)
            pcmDataSamples = [
                ExtractAndEncodeData(inputFolder + "k9_affirmative.wav"),
                ExtractAndEncodeData(inputFolder + "k9_insufficient_data.wav"),
                ExtractAndEncodeData(inputFolder + "tardis_door.wav")];

            WriteData(startIndices, endIndices, pcmDataSamples, outputFolder + "pcm_secondary.dat");

            // write address tables (copied to sound.6502)
            Console.WriteLine("Paste following lines into relocateSampleParams at the bottom of level.6502");
            WriteAddressTable("pcmDataStartAddrLoTblLocal", startIndices, (int value) => LO(value));
            WriteAddressTable("pcmDataStartAddrHiTblLocal", startIndices, (int value) => HI(value));
            WriteAddressTable("pcmDataEndAddrHiTblLocal  ", endIndices, (int value) => HI(value));

            // create 4-bit resolution WAV files for comparison
            // ConvertWAVtoWAV(inputFolder + "dalek_exterminate.wav", inputFolder + "dalek_exterminate_4bit.wav");
            // ConvertWAVtoWAV(inputFolder + "dalek_groan.wav", inputFolder + "dalek_groan_4bit.wav");
            // ConvertWAVtoWAV(inputFolder + "dalek_weapon.wav", inputFolder + "dalek_weapon_4bit.wav");
            // ConvertWAVtoWAV(inputFolder + "k9_affirmative.wav", inputFolder + "k9_affirmative_4bit.wav");
            // ConvertWAVtoWAV(inputFolder + "k9_insufficient_data.wav", inputFolder + "k9_insufficient_data_4bit.wav");
            // ConvertWAVtoWAV(inputFolder + "tardis_door.wav", inputFolder + "tardis_door_4bit.wav");
        }

        private static void WriteAddressTable(string symbolName, List<int> addresses, Func<int, int> func)
        {
            Console.Write($".{symbolName} EQUB ");
            for (int i = 0; i < addresses.Count; i++)
            {
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write($"&{func(addresses[i]):X2}");
            }
            Console.WriteLine();
        }

        private static void WriteData(List<int> startIndices, List<int> endIndices, byte[][] pcmDataSamples, string filePath)
        { 
            // stack encoded data, aligning ends to page boundaries
            byte[] pcmData = new byte[fileSize];

            int offset = fileSize;
            for (int i = 0; i < pcmDataSamples.Length; i++)
            {
                int sampleLength = pcmDataSamples[i].Length;

                Array.Copy(pcmDataSamples[i], 0, pcmData, offset - sampleLength, sampleLength);

                startIndices.Add(offset - sampleLength);
                endIndices.Add(offset);

                offset -= RoundToNextPage(sampleLength);
            }

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
            using FileStream fileStream = File.OpenWrite(filePath);

            // write data
            fileStream.Write(pcmData);
        }

        private static int RoundToNextPage(int value) => (value + 255) / 256 * 256;
        private static int HI(int value) => (0x8000 + value) / 256;
        private static int LO(int value) => (0x8000 + value) % 256;

        private static byte[] ExtractAndEncodeData(string filePath)
        {
            // read PCM data from 8/8K WAV file
            byte[] pcmData = ReadPCMData(filePath, out _);
            if (pcmData.Length == 0) return [];

            // encode the data
            int halfLength = pcmData.Length / 2;
            byte[] encodedData = new byte[halfLength];

            for (int i = 0; i < halfLength; i++)
            {
                // convert two 8-bit samples to two 4-bit SN76489 volume values
                int firstHalfAmplitude = SampleToVolumeRegister(pcmData[i]);
                int secondHalfAmplitude = SampleToVolumeRegister(pcmData[i + halfLength]);

                // write the two 4-bit volume values to the low and high nibbles
                encodedData[i] = (byte)(firstHalfAmplitude | (secondHalfAmplitude << 4));
            }

            return encodedData;
        }

        private static int SampleToVolumeRegister(int value)
        {
            // non-linear lookup
            for (int i = 0; i < VolumeToRegisterLookup.Length; i++)
            {
                if (value >= VolumeToRegisterLookup[i])
                {
                    return i;
                }
            }

            return 0;
        }

        private static readonly int[] VolumeToRegisterLookup = [204, 162, 129, 103, 82, 65, 52, 41, 33, 27, 21, 17, 14, 11, 8, 0];

        private static byte[] ReadPCMData(string filePath, out long pcmPosition)
        {
            FileStream fileStream = File.OpenRead(filePath);

            pcmPosition = 0;

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
            pcmPosition = fileStream.Position;
            fileStream.ReadExactly(pcmData);

            return pcmData;
        }

        private static short ReadShort(FileStream fileStream)
        {
            return (short)(fileStream.ReadByte() | (fileStream.ReadByte() << 8));
        }

        private static int ReadInt(FileStream fileStream)
        {
            return fileStream.ReadByte() | (fileStream.ReadByte() << 8) | (fileStream.ReadByte() << 16) | (fileStream.ReadByte() << 24);
        }

        private static string GetSolutionFolder()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && directory.GetFiles("*.sln").Length == 0)
            {
                directory = directory.Parent;
            }
            return (directory != null) ? directory.FullName : string.Empty;
        }

        private static void ConvertWAVtoWAV(string inputFilePath, string outputFilePath)
        {
            byte[] pcmData = ReadPCMData(inputFilePath, out long pcmPosition);

            for (int i = 0; i < pcmData.Length; i++)
            {
                // re-quantize to 4-bits
                pcmData[i] = (byte)(pcmData[i] & 0xF8);
            }

            File.Copy(inputFilePath, outputFilePath, overwrite:true);

            using FileStream fs = File.OpenWrite(outputFilePath);

            fs.Seek(pcmPosition, SeekOrigin.Begin);
            fs.Write(pcmData);
        }
    }
}
