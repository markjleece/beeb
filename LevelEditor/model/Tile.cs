// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    class Tile
    {
        internal Tile Clone()
        {
            Tile clone = new();

            clone.Type = Type;

            for (int i = 0; i < TileWidth * TileHeight; i++)
            {
                clone.Pixels[i] = Pixels[i];
            }

            return clone;
        }

        internal void Read(FileStream fs)
        {
            const int size = TileWidth * TileHeight / 4;
            const int halfSize = size / 2;

            byte[] buffer = new byte[size];
            fs.ReadExactly(buffer, 0, size);

            int[] highPixelMask = [ 0x80, 0x40, 0x20, 0x10 ];
            int[] lowPixelMask = [ 0x08, 0x04, 0x02, 0x01 ];
            int[] highPixelShift = [ 6, 5, 4, 3 ];
            int[] lowPixelShift = [ 3, 2, 1, 0 ];

            for (int y = 0; y < TileHeight; y++)
            {
                for (int x = 0; x < TileWidth; x++)
                {
                    int rowIndex = y / 8;
                    int subRowIndex = y % 8;
                    int columnIndex = x / 4;
                    int subColumnIndex = x % 4;

                    int bufferIndex = (rowIndex * halfSize) + (8 * columnIndex) + subRowIndex;
                    byte pixelQuad = buffer[bufferIndex];

                    int pixel = ((pixelQuad & highPixelMask[subColumnIndex]) >> highPixelShift[subColumnIndex]) |
                                ((pixelQuad & lowPixelMask[subColumnIndex]) >> lowPixelShift[subColumnIndex]);

                    Pixels[y * 16 + x] = (byte)pixel;
                }
            }

            // set default tile type
            Type = TileType.Space;
        }

        internal void Write(FileStream fs)
        {
            const int size = TileWidth * TileHeight / 4;
            const int halfSize = size / 2;
            byte[] buffer = new byte[size];

            int[] highPixelShift = [ 6, 5, 4, 3 ];
            int[] lowPixelShift = [ 3, 2, 1, 0 ];

            for (int y = 0; y < TileHeight; y++)
            {
                for (int x = 0; x < TileWidth; x++)
                {
                    int pixel = Pixels[y * 16 + x];

                    int rowIndex = y / 8;
                    int subRowIndex = y % 8;
                    int columnIndex = x / 4;
                    int subColumnIndex = x % 4;

                    int pixelQuad = ((pixel & 0x1) << lowPixelShift[subColumnIndex]) |
                                    ((pixel & 0x2) << highPixelShift[subColumnIndex]);

                    int bufferIndex = (rowIndex * halfSize) + (8 * columnIndex) + subRowIndex;
                    buffer[bufferIndex] |= (byte)pixelQuad;
                }
            }

            fs.Write(buffer, 0, size);
        }

        internal bool Validate()
        {
            foreach (int value in Pixels)
            {
                if (value > 3)
                {
                    return false; // invalid value
                }
            }

            return true;
        }

        internal byte[] GetScaledARGB(int width, int height, uint[] palette)
        {
            byte[] scaledPixels = new byte[4 * width * height];

            int deltaX = (16 << 16) / width;
            int deltaY = (16 << 16) / height;

            int idx = 0;

            for (int i = height, accumY = 0; i != 0; i--)
            {
                int yMul16 = (accumY >> 16) * 16;
                accumY += deltaY;

                for (int j = width, accumX = 0; j != 0; j--)
                {
                    int x = (accumX >> 16);
                    accumX += deltaX;

                    int paletteIndex = (byte)Pixels[yMul16 + x];
                    uint argb = palette[paletteIndex];

                    scaledPixels[idx++] = (byte)(argb >> 0);
                    scaledPixels[idx++] = (byte)(argb >> 8);
                    scaledPixels[idx++] = (byte)(argb >> 16);
                    scaledPixels[idx++] = (byte)(argb >> 24);
                }
            }

            return scaledPixels;
        }

        internal byte this[int x, int y]
        {
            get => Pixels[y * 16 + x];
            set => Pixels[y * 16 + x] = value;
        }

        internal const int OpaqueFlag = 0x80;

        internal enum TileType
        {
            Space = 1,
            Wall = OpaqueFlag + 0,
            Floor = OpaqueFlag + 1,
            Key = OpaqueFlag + 2,
            Jewel = OpaqueFlag + 3,
            Door = OpaqueFlag + 4,
            Elevator = OpaqueFlag + 5,
            Exit = OpaqueFlag + 6
        }

        internal TileType Type = TileType.Space;

        internal byte[] Pixels = new byte[TileWidth * TileHeight];

        private const int TileWidth = 16;
        private const int TileHeight = 16;
    }
}
