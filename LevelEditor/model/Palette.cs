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

namespace LevelEditor
{
    class Palette
    {
        internal Palette()
        {
            ColorIndices = new byte[PaletteSize];
            ColorIndices[0] = 7; // white
            ColorIndices[1] = 4; // blue 
            ColorIndices[2] = 3; // yellow
            ColorIndices[3] = 0; // black
        }

        internal Palette Clone()
        {
            Palette clone = new();
            for (int i = 0; i < PaletteSize; i++)
            {
                clone.ColorIndices[i] = ColorIndices[i];
            }
            return clone;
        }

        internal void Read(FileStream fs)
        {
            // read BBC 4-color palette format
            for (int i = 0; i < PaletteSize; i++)
            {
                ColorIndices[i] = (byte)((fs.ReadByte() ^ 0x07) & 0x0F);
                fs.ReadByte(); // skip
                fs.ReadByte(); // skip
                fs.ReadByte(); // skip
            }
        }

        internal void Write(FileStream fs)
        {
            // write BBC 4-color palette format
            byte[] logicalColors = [0x00, 0x20, 0x80, 0xA0];
            for (int i = 0; i < PaletteSize; i++)
            {
                byte logicalColor = logicalColors[i];
                byte physicalColor = (byte)((ColorIndices[i] ^ 0x07) & 0x0F);
                fs.WriteByte((byte)(logicalColor | 0x00 | physicalColor));
                fs.WriteByte((byte)(logicalColor | 0x10 | physicalColor));
                fs.WriteByte((byte)(logicalColor | 0x40 | physicalColor));
                fs.WriteByte((byte)(logicalColor | 0x50 | physicalColor));
            }
        }

        internal bool Validate()
        {
            foreach (int value in ColorIndices)
            {
                if (value > 7)
                {
                    return false; // invalid value
                }
            }

            return true;
        }

        internal byte this[int i]
        {
            get => ColorIndices[i];
            set => ColorIndices[i] = value;
        }

        internal byte[] ColorIndices;

        private const int PaletteSize = 4;
    }
}
