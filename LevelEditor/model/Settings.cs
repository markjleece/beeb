// This file is Copyright © 2025 - Mark John Leece - All rights reserved
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
                SampleSoundsEnabled = SampleSoundsEnabled
            };
            return clone;
        }

        internal void Read(FileStream fs)
        {
            EnemyWeaponStrength = fs.ReadByte() * 4; // serialized value is divided by four
            LaserStrength = fs.ReadByte();
            LaserHealthDrain = fs.ReadByte();
            JewelHealthGain = fs.ReadByte() * 256;
            SampleSoundsEnabled = fs.ReadByte();
        }

        internal void Write(FileStream fs)
        {
            fs.WriteByte((byte)(EnemyWeaponStrength / 4)); // serialized value is divided by four
            fs.WriteByte((byte)LaserStrength);
            fs.WriteByte((byte)LaserHealthDrain);
            fs.WriteByte((byte)(JewelHealthGain / 256));
            fs.WriteByte((byte)SampleSoundsEnabled);
        }

        internal int EnemyWeaponStrength = 256;
        internal int JewelHealthGain = 512;
        internal int LaserHealthDrain = 8;
        internal int LaserStrength = 32;
        internal int SampleSoundsEnabled = 1/*true*/;
    }
}
