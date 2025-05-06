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
            Settings clone = new Settings();
            clone.EnemyWeaponStrength = EnemyWeaponStrength;
            clone.JewelHealthGain = JewelHealthGain;
            clone.LaserHealthDrain = LaserHealthDrain;
            clone.LaserStrength = LaserStrength;
            clone.SampleBasedSounds = SampleBasedSounds;
            return clone;
        }

        internal void Read(FileStream fs)
        {
            EnemyWeaponStrength = fs.ReadByte();
            LaserStrength = fs.ReadByte();
            LaserHealthDrain = fs.ReadByte();
            JewelHealthGain = fs.ReadByte() + fs.ReadByte() * 256;
            SampleBasedSounds = fs.ReadByte();
        }

        internal void Write(FileStream fs)
        {
            fs.WriteByte((byte)EnemyWeaponStrength);
            fs.WriteByte((byte)LaserStrength);
            fs.WriteByte((byte)LaserHealthDrain);
            fs.WriteByte((byte)(JewelHealthGain % 256));
            fs.WriteByte((byte)(JewelHealthGain / 256));
            fs.WriteByte((byte)SampleBasedSounds);
        }

        internal int EnemyWeaponStrength = 64;
        internal int JewelHealthGain = 256;
        internal int LaserHealthDrain = 8;
        internal int LaserStrength = 32;
        internal int SampleBasedSounds = 1/*true*/;
    }
}
