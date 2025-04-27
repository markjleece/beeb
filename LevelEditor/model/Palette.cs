// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor
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
            Palette clone = new Palette();
            for (int i = 0; i < PaletteSize; i++)
            {
                clone.ColorIndices[i] = ColorIndices[i];
            }
            return clone;
        }

        internal void Read(FileStream fs)
        {
            // read bbc 4-color palette format
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
            // write bbc 4-color palette format
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
