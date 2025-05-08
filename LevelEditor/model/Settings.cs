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
            clone.DalekSamplesEnabled = DalekSamplesEnabled;
            return clone;
        }

        internal void Read(FileStream fs)
        {
            EnemyWeaponStrength = fs.ReadByte();
            LaserStrength = fs.ReadByte();
            LaserHealthDrain = fs.ReadByte();
            JewelHealthGain = fs.ReadByte() * 256;
            DalekSamplesEnabled = fs.ReadByte();
        }

        internal void Write(FileStream fs)
        {
            fs.WriteByte((byte)EnemyWeaponStrength);
            fs.WriteByte((byte)LaserStrength);
            fs.WriteByte((byte)LaserHealthDrain);
            fs.WriteByte((byte)(JewelHealthGain / 256));
            fs.WriteByte((byte)DalekSamplesEnabled);
        }

        internal int EnemyWeaponStrength = 64;
        internal int JewelHealthGain = 512;
        internal int LaserHealthDrain = 8;
        internal int LaserStrength = 32;
        internal int DalekSamplesEnabled = 1/*true*/;
    }
}
