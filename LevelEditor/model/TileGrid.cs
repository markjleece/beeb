// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LevelEditor
{
    class TileGrid
    {
        internal TileGrid()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;

            TileIndices = new byte[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                TileIndices[i] = 0;
            }
        }

        internal TileGrid Clone()
        {
            TileGrid clone = new()
            {
                Width = Width,
                Height = Height
            };

            for (int i = 0; i < Width * Height; i++)
            {
                clone.TileIndices[i] = TileIndices[i];
            }

            return clone;
        }

        internal void Read(FileStream fs)
        {
            fs.ReadExactly(TileIndices);

            for (int i = 0; i < Width * Height; i++)
            {
                TileIndices[i] = (byte)(TileIndices[i] & TileIndexMask);
            }
        }

        internal void Write(FileStream fs, Tile[] tiles)
        {
            // write tile indices, embedding opacity
            for (int i = 0; i < Width * Height; i++)
            {
                int index = TileIndices[i];

                // ensure object tiles are treated as space
                if (index >= Level.TileCount)
                {
                    index = 0;
                }

                // apply opacity flag
                int flags = (int)tiles[index].Type & Tile.OpaqueFlag;

                fs.WriteByte((byte)(flags | index));
            }
        }

        internal bool Validate()
        {
            if ((Width * Height) != (DefaultWidth * DefaultHeight))
            {
                return false; // invalid extents
            }

            foreach (int tileIndex in TileIndices)
            {
                if (tileIndex < 0 || tileIndex > 37)
                {
                    return false; // invalid index
                }
            }
            return true;
        }

        internal void Resize(int newWidth, int newHeight)
        {
            // preserve overlaying area
            int minWidth = Math.Min(Width, newWidth);
            int minHeight = Math.Min(Height, newHeight);

            byte[] newTileIndices = new byte[newWidth * newHeight];
            for (int y = 0; y < minHeight; y++)
            {
                for (int x = 0; x < minWidth; x++)
                {
                    newTileIndices[y * newWidth + x] = TileIndices[y * Width + x];
                }
            }

            TileIndices = newTileIndices;
            Width = newWidth;
            Height = newHeight;
        }

        internal byte this[int x, int y]
        {
            get => TileIndices[y * Width + x];
            set => TileIndices[y * Width + x] = value;
        }

        internal int GetRuntimeAddress(int tileX, int tileY)
        {
            return RuntimeAddress + (tileY * Width) + tileX;
        }

        private const int DefaultWidth = 4 * 16;
        private const int DefaultHeight = 2 * 16;
        private const int TileIndexMask = 0x1F;
        private const int TileFlagsMask = 0xE0;
        private const int RuntimeAddress = 0x3800;

        internal int Width;
        internal int Height;
        internal byte[] TileIndices;

        // special indices
        internal const int K9LookLeft = Level.TileCount + 0;
        internal const int K9LookRight = Level.TileCount + 1;
        internal const int EnemyLookLeft = Level.TileCount + 2;
        internal const int EnemyLookRight = Level.TileCount + 3;
        internal const int EnemyMoveLeft = Level.TileCount + 4;
        internal const int EnemyMoveRight = Level.TileCount + 5;
    }
}
