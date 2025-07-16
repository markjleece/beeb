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
    class Settings
    {
        public Settings()
        {
        }

        internal Settings Clone()
        {
            Settings clone = new()
            {
                EnemyWeaponStrength = EnemyWeaponStrength,
                JewelHealthGain = JewelHealthGain,
                LaserHealthDrain = LaserHealthDrain,
                LaserStrength = LaserStrength,
                SampleSoundsEnabled = SampleSoundsEnabled,
                WeepingAngels = WeepingAngels,
            };
            return clone;
        }

        internal void Read(FileStream fs)
        {
            EnemyWeaponStrength = fs.ReadByte() * 4; // serialized value is divided by four
            LaserStrength = fs.ReadByte();
            LaserHealthDrain = fs.ReadByte();
            JewelHealthGain = fs.ReadByte() * 256; // high-byte serialized
            int flags = fs.ReadByte();
            SampleSoundsEnabled = (flags & SAMPLE_SOUNDS_FLAG) != 0;
            WeepingAngels = (flags & WEEPING_ANGELS_FLAG) != 0;
        }

        internal void Write(FileStream fs)
        {
            fs.WriteByte((byte)(EnemyWeaponStrength / 4)); // serialized value is divided by four
            fs.WriteByte((byte)LaserStrength);
            fs.WriteByte((byte)LaserHealthDrain);
            fs.WriteByte((byte)(JewelHealthGain / 256)); // high-byte serialized
            int flags = 0;
            flags |= SampleSoundsEnabled ? SAMPLE_SOUNDS_FLAG : 0;
            flags |= WeepingAngels ? WEEPING_ANGELS_FLAG : 0;
            fs.WriteByte((byte)flags);
        }

        internal int EnemyWeaponStrength = 256;
        internal int JewelHealthGain = 512;
        internal int LaserHealthDrain = 8;
        internal int LaserStrength = 32;
        internal bool SampleSoundsEnabled = true;
        internal bool WeepingAngels = false;

        private const int SAMPLE_SOUNDS_FLAG = 0x01;
        private const int WEEPING_ANGELS_FLAG = 0x02;
    }
}
