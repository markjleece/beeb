// This file is Copyright © 2025 - Mark John Leece - All rights reserved
//
// This console app converts 32-bit BMP level assets (created in paint.net) into level altas
// files, which are then packaged with level.6502 files to create level files within the SSD.
//
// The app should be run after any BMP asset file changes are made, followed by a rebuild
// of the game itself; so the new level atlas files are repackaged with the game.
//
// Each level's BMP assets are held in a folder named assets\levelN, where N is '1'..'3'
// The asset file names, dimensions, and sprite counts can be found below.
//
// Note that the colors used within the BMP asset files must be a close match to the colors
// defined in the associated level's palette.
// 
using System.Numerics;

namespace BMPConverter
{
    internal delegate void Serializer(FileStream fs);

    internal class Program
    {
        const char LevelCount = '6';

        private static readonly string assetsFolderPath = GetSolutionFolder() + "\\Game\\assets\\level{0}\\";
        private static readonly string dataFolderPath = GetSolutionFolder() + "\\Game\\data\\";

        static void Main(string[] _)
        {
            try
            {
                for (char level = '1'; level <= LevelCount; level++)
                {
                    Color[] palette = ReadLevelPalette(level);

                    string outputFileName = dataFolderPath + $"level{level}.atl";
                    try
                    {
                        // delete existing file as File.OpenWrite(...) does will open an existing file for write
                        File.Delete(outputFileName);
                    }
                    catch (Exception ex)
                    {
                        if (ex is not FileNotFoundException)
                        {
                            throw new Exception($"Error deleting: {outputFileName}");
                        }
                    }

                    using FileStream fs = File.OpenWrite(outputFileName);
                    {
                        string folderPath = string.Format(assetsFolderPath, level);
                        byte[] data;

                        data = ConvertBMPFile(folderPath + "enemy_left.bmp", spriteCount: 4, spriteWidth: 24, spriteHeight: 32, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "enemy_right.bmp", spriteCount: 4, spriteWidth: 24, spriteHeight: 32, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "enemy_turn.bmp", spriteCount: 3, spriteWidth: 24, spriteHeight: 32, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "door.bmp", spriteCount: 4, spriteWidth: 4, spriteHeight: 32, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "elevator.bmp", spriteCount: 4, spriteWidth: 32, spriteHeight: 3, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "teleport.bmp", spriteCount: 4, spriteWidth: 32, spriteHeight: 3, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "k9_left.bmp", spriteCount: 4, spriteWidth: 24, spriteHeight: 16, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "k9_right.bmp", spriteCount: 4, spriteWidth: 24, spriteHeight: 16, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "k9_turn.bmp", spriteCount: 3, spriteWidth: 24, spriteHeight: 16, palette);
                        fs.Write(data, 0, data.Length);

                        data = ConvertBMPFile(folderPath + "gamebar.bmp", spriteCount: 1, spriteWidth: 112, spriteHeight: 8, palette);
                        fs.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }
        }

        //
        // Read level palette
        //
        static private Color[] ReadLevelPalette(char level)
        {
            Color[] palette = new Color[4];

            // read palette data from level file
            FileStream fileStream = File.OpenRead(dataFolderPath + $"level{level}.dat");

            byte[] paletteData = new byte[16];
            fileStream.ReadExactly(paletteData);
            fileStream.Close();

            // extract palette colors
            for (int i = 0; i < 4; i++)
            {
                palette[i] = (Color)((paletteData[i * 4] & 0x7) ^ 0x7);
            }

            return palette;
        }

        //
        // Convert BMP file into bbc sprite data (column based, 4-color data)
        //
        static private byte[] ConvertBMPFile(string bmpFilePathName, int spriteCount, int spriteWidth, int spriteHeight, Color[] palette)
        {
            FileStream fileStream = File.OpenRead(bmpFilePathName);

            // read file header
            if (fileStream.ReadByte() != 'B' || fileStream.ReadByte() != 'M')
            {
                Console.WriteLine("Error, not a BMP file!");
                return [];
            }

            ReadInt(fileStream); // skip bmpSize

            ReadShort(fileStream); // skip reserved
            ReadShort(fileStream); // skip reserved

            int pixelDataOffset = ReadInt(fileStream);

            // read DIB header
            int DIBHeaderSize = ReadInt(fileStream);
            if (DIBHeaderSize != 124)
            {
                Console.WriteLine("Error, unsupported DIB header!");
                return [];
            }

            int bitmapWidth = ReadInt(fileStream);
            int bitmapHeight = ReadInt(fileStream);
            short colorPlanes = ReadShort(fileStream);
            short bitsPerPixel = ReadShort(fileStream);
            ReadInt(fileStream); // skip compressionMethod
            ReadInt(fileStream); // skip imageSizeInBytes
            ReadInt(fileStream); // skip horizontal Res
            ReadInt(fileStream); // skip vertical Res
            int logicalColorCount = ReadInt(fileStream);
            ReadInt(fileStream); // skip importantColorCount

            if (bitsPerPixel != 32 || colorPlanes != 1 || logicalColorCount != 0)
            {
                Console.WriteLine("Error, not a 32 bits-per-pixel image!");
                return [];
            }

            if (bitmapWidth != spriteCount * spriteWidth || bitmapHeight != spriteHeight)
            {
                Console.WriteLine("Error, image extents do not match expectation!");
                return [];
            }

            // read bit-field masks
            int redMask = ReadInt(fileStream);
            int greenMask = ReadInt(fileStream);
            int blueMask = ReadInt(fileStream);

            // calc bit-field shifts
            int redShift = BitOperations.TrailingZeroCount(redMask);
            int greenShift = BitOperations.TrailingZeroCount(greenMask);
            int blueShift = BitOperations.TrailingZeroCount(blueMask);

            // read image data
            int[] bitmapData = new int[bitmapWidth * bitmapHeight];

            fileStream.Seek(pixelDataOffset, SeekOrigin.Begin);
            for (int i = 0; i < bitmapWidth * bitmapHeight; i++)
            {
                bitmapData[i] = ReadInt(fileStream);
            }

            int index = 0;
            byte[] result = new byte[bitmapWidth * bitmapHeight / 4];
            byte[][] bbcScreenBits = [[0x00, 0x08, 0x80, 0x88], [0x00, 0x04, 0x40, 0x44], [0x00, 0x02, 0x20, 0x22], [0x00, 0x01, 0x10, 0x11]];

            for (int x = 0; x < bitmapWidth; x += 4)
            {
                for (int y = bitmapHeight - 1; y >= 0; y--)
                {
                    int idx = (y * bitmapWidth) + x; // pixel index

                    // construct bbcScreenByte (four spliced pixels)
                    byte bbcScreenByte = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        int rgba = bitmapData[idx + i];
                            byte bbcLogicalColor = RGBAtoLogicalColor(rgba, redMask, greenMask, blueMask, redShift, greenShift, blueShift, palette);
                        bbcScreenByte |= bbcScreenBits[i][bbcLogicalColor];
                    }

                    // output screen byte, buffered
                    result[index++] = (byte)bbcScreenByte;
                }
            }

            return result;
        }
         
        private static byte RGBAtoLogicalColor(
            int rgba, 
            int redMask, int greenMask, int blueMask, 
            int redShift, int greenShift, int blueShift,
            Color[] palette)
        {
            // extract color components
            int red = (rgba & redMask) >> redShift;
            int green = (rgba & greenMask) >> greenShift;
            int blue = (rgba & blueMask) >> blueShift;

            // match against palette entries
            for (byte i = 0; i < 4; i++)
            {
                switch (palette[i])
                {
                    case Color.Black:
                        if (red < 64 && green < 64 && blue < 64) return i;
                        break;

                    case Color.Red:
                        if (red > 192 && green < 64 && blue < 64) return i;
                        break;

                    case Color.Green:
                        if (red < 64 && green > 192 && blue < 64) return i;
                        break;

                    case Color.Yellow:
                        if (red > 192 && green > 192 && blue < 64) return i;
                        break;

                    case Color.Blue: 
                        if (red < 64 && green < 64 && blue > 192) return i;
                        break;

                    case Color.Magenta:
                        if (red > 192 && green < 64 && blue > 192) return i;
                        break;

                    case Color.Cyan:
                        if (red < 64 && green > 192 && blue > 192) return i;
                        break;

                    case Color.White:
                        if (red > 192 && green > 192 && blue > 192) return i;
                        break;

                    default:
                        break;
                }
            }

            Console.WriteLine($"Error unsupported color: R={red}, G={green}, B={blue}");
            return 0;
        }

        private static short ReadShort(FileStream fileStream)
        {
            return (short)(fileStream.ReadByte() | (fileStream.ReadByte() << 8));
        }

        private static int ReadInt(FileStream fileStream)
        {
            return fileStream.ReadByte() | (fileStream.ReadByte() << 8) | (fileStream.ReadByte() << 16) | (fileStream.ReadByte() << 24);
        }

        // bbc logical colors
        enum Color : byte
        {
            Black = 0,
            Red = 1,
            Green = 2,
            Yellow = 3,
            Blue = 4,
            Magenta = 5,
            Cyan = 6,
            White = 7
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
