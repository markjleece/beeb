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
    class Object
    {
        internal Object(int type, int posX, int posY, int data = 0)
        {
            Type = type;
            PosX = posX;
            PosY = posY;
            Data = data;
        }

        internal Object Clone()
        {
            return new Object(Type, PosX, PosY, Data);
        }

        internal void Read(FileStream fs)
        {
            Type = fs.ReadByte();
            if (Type == -1)
            {
                throw new Exception("Unexpected end-of-stream");
            }

            if (Type == Object.Type_DisabledDoor)
            {
                Type = Object.Type_Door;
            }
            else if (Type == Object.Type_DisabledElevator)
            {
                Type = Object.Type_Elevator;
            }

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

            Data = fs.ReadByte();
            if (Data == -1)
            {
                throw new Exception("Unexpected end-of-stream");
            }
        }

        internal void Write(FileStream fs)
        {
            fs.WriteByte((byte)Type);
            fs.WriteByte((byte)PosX);
            fs.WriteByte((byte)PosY);
            fs.WriteByte((byte)Data);
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

        // disabled elevator and door
        internal const int Type_DisabledElevator = 0x40 | Type_Elevator;
        internal const int Type_DisabledDoor = 0x40 | Type_Door;

        // bits 2..4
        internal const int Animate_Mask = 0x1C;
        internal const int Animate_LookLeft = 0 << 2;
        internal const int Animate_LookRight = 1 << 2;
        internal const int Animate_MoveLeft = 2 << 2;
        internal const int Animate_MoveRight = 3 << 2;

        internal int PosX;
        internal int PosY;
        internal int Type;
        internal int Data;

        internal const int MaxEnemyCount = 15;
    }
}
