// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    class Object
    {
        internal Object(int posX, int posY, int type)
        {
            PosX = posX;
            PosY = posY;
            Type = type;
        }

        internal Object Clone()
        {
            return new Object(PosX, PosY, Type);
        }

        internal void Read(FileStream fs)
        {
            PosX = fs.ReadByte();
            if (PosX == -1)
            {
                throw new Exception("Unexpected end-of-stream");
            }

            PosY = fs.ReadByte();
            if (PosY == -1)
            {
                throw new Exception("Unexpected end-of-stream");
            }

            Type = fs.ReadByte();
            if (Type == -1)
            {
                throw new Exception("Unexpected end-of-stream");
            }
        }

        internal void Write(FileStream fs)
        {
            fs.WriteByte((byte)PosX);
            fs.WriteByte((byte)PosY);
            fs.WriteByte((byte)Type);
        }

        internal bool Validate()
        {
            if (PosX % 16 != 0 || PosY % 16 != 0)
            {
                return false;
            }

            if (Type == Type_Unknown || Type == Type_Elevator || Type == Type_Door)
            {
                return true; 
            }

            if (Type == (Type_K9 | Animate_LookLeft) || 
                Type == (Type_K9 | Animate_LookRight))
            {
                return true;
            }

            if (Type == (Type_Enemy | Animate_LookLeft) || 
                Type == (Type_Enemy | Animate_LookRight) || 
                Type == (Type_Enemy | Animate_MoveLeft) || 
                Type == (Type_Enemy | Animate_MoveRight))
            {
                return true;
            }

            return false;
        }

        internal const int Type_Unknown = 0x80;

        // bits 0,1
        internal const int TypeMask = 0x03;
        internal const int Type_K9 = 0;
        internal const int Type_Enemy = 1;
        internal const int Type_Elevator = 2;
        internal const int Type_Door = 3;

        // bits 2..4
        internal const int Animate_Mask = 0x1C;
        internal const int Animate_LookLeft = 0 << 2;
        internal const int Animate_LookRight = 1 << 2;
        internal const int Animate_MoveLeft = 2 << 2;
        internal const int Animate_MoveRight = 3 << 2;

        internal int PosX;
        internal int PosY;
        internal int Type;

        internal const int MaxEnemyCount = 15;
    }
}
