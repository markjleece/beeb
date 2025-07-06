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

using System.Diagnostics;

namespace LevelEditor
{
    class Level
    {
        internal string FilePathName;
        internal Palette Palette;
        internal TileGrid TileGrid;
        internal Tile[] Tiles;
        internal Settings Settings;
        internal TeleportGrid TeleportGrid;
        internal SwitchGrid SwitchGrid;

        internal const int ObjectCount = 48;
        internal const int TileCount = 28;

        internal Level()
        {
            FilePathName = string.Empty;
            Palette = new Palette();
            Settings = new Settings();
            TeleportGrid = new TeleportGrid();
            SwitchGrid = new SwitchGrid();
            TileGrid = new TileGrid();
            Tiles = new Tile[TileCount];
            for (int i = 0; i < TileCount; i++)
            {
                Tiles[i] = new();
            }
        }

        internal Level Clone()
        {
            Level clone = new()
            {
                FilePathName = FilePathName,
                Palette = Palette.Clone(),
                Settings = Settings.Clone(),
                TeleportGrid = TeleportGrid.Clone(),
                SwitchGrid = SwitchGrid.Clone(),
                TileGrid = TileGrid.Clone(),
            };

            for (int i = 0; i < TileCount; i++)
            {
                clone.Tiles[i] = Tiles[i].Clone();
            }

            return clone;
        }

        internal void Set(Level other)
        {
            other = other.Clone();

            FilePathName = other.FilePathName;
            Palette = other.Palette;
            Settings = other.Settings;
            TeleportGrid = other.TeleportGrid;
            SwitchGrid = other.SwitchGrid;
            TileGrid = other.TileGrid;
            Tiles = other.Tiles;
        }

        internal void Read(FileStream fs)
        {
            // 16 byte palette
            Palette.Read(fs);

            // 27 x 64 byte tiles
            for (int i = 1; i < TileCount; i++)
            {
                Tiles[i].Read(fs);
            }

            // 2K tile grid
            TileGrid.Read(fs);

            // tile grid width & height
            TileGrid.Width = fs.ReadByte();
            TileGrid.Height = fs.ReadByte();

            // 28 x 1 byte tile types
            byte[] tileTypes = new byte[TileCount];
            fs.ReadExactly(tileTypes);

            // set tile types
            for (int i = 0; i < TileCount; i++)
            {
                Tiles[i].Type = (Tile.TileType)tileTypes[i];
            }

            // settings (5 bytes)
            Settings.Read(fs);

            // 48 x 4 byte objects
            Object[] objects = new Object[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                objects[i] = new Object(Object.Type_Unknown, 0, 0);
                objects[i].Read(fs);
            }

            // switch pairings (32 bytes)
            SwitchGrid.Read(fs, objects, Tiles, TileGrid);

            PropagateObjects(objects);
        }

        internal void Write(FileStream fs)
        {
            // 16 byte palette (loaded @ 30B0)
            Palette.Write(fs);

            // 27 x 64 byte tiles
            for (int i = 1; i < TileCount; i++)
            {
                Tiles[i].Write(fs);
            }

            // 2K tile grid
            TileGrid.Write(fs, Tiles);

            // tile grid width & height
            fs.WriteByte((byte)TileGrid.Width);
            fs.WriteByte((byte)TileGrid.Height);

            // 28 x 1 byte tile types
            foreach (Tile tile in Tiles)
            {
                fs.WriteByte((byte)tile.Type);
            }

            // settings (5 bytes)
            Settings.Write(fs);

            // 48 x 4 byte objects
            Object[] objects = CollectObjects();
            Object empty = new(Object.Type_Unknown, 0, 0);
            for (int i = 0; i < ObjectCount; i++)
            {
                if (i < objects.Length)
                {
                    objects[i].Write(fs);
                }
                else
                {
                    empty.Write(fs);
                }
            }

            // switch pairings (32 bytes)
            SwitchGrid.Write(fs, objects, Tiles, TileGrid);

            if (objects[0].Type == Object.Type_Unknown)
            {
                MessageBox.Show("K9 has not been placed within the level!\n\n" + 
                                "The level will not work within the game.",
                                "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (objects.Length > ObjectCount)
            {
                MessageBox.Show("The maximum number of objects (k9 + enemies + elevators + doors) has been exceeded!\n\n" +
                                $"The first { ObjectCount } objects are saved.",
                                "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal int GetTileIndex(Tile.TileType tileType)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i].Type == tileType)
                {
                    return i;
                }
            }
            return -1;
        }

        internal Object[] CollectObjects()
        {
            List<Object> objects =
            [
                // create first slot for k9 (will be populated later)
                new Object(Object.Type_Unknown, 0, 0),
            ];

            //
            // first pass - collect k9 & enemies
            //
            for (int tileY =  0; tileY < TileGrid.Height; tileY++)
            {
                for (int tileX = 0; tileX < TileGrid.Width; tileX++)
                {
                    int tileIndex = TileGrid[tileX, tileY];
                    if (tileIndex < Level.TileCount)
                    {
                        continue;
                    }

                    int posX = tileX;
                    int posY = tileY;

                    switch (tileIndex)
                    {
                        case TileGrid.K9LookLeft:
                            objects[0] = new Object(Object.Type_K9 | Object.Animate_LookLeft, posX, posY);
                            break;

                        case TileGrid.K9LookRight:
                            objects[0] = new Object(Object.Type_K9 | Object.Animate_LookRight, posX, posY);
                            break;

                        case TileGrid.EnemyLookLeft:
                            objects.Add(new Object(Object.Type_Enemy | Object.Animate_LookLeft, posX, posY - 1));
                            break;

                        case TileGrid.EnemyLookRight:
                            objects.Add(new Object(Object.Type_Enemy | Object.Animate_LookRight, posX, posY - 1));
                            break;

                        case TileGrid.EnemyMoveLeft:
                            objects.Add(new Object(Object.Type_Enemy | Object.Animate_MoveLeft, posX, posY - 1));
                            break;

                        case TileGrid.EnemyMoveRight:
                            objects.Add(new Object(Object.Type_Enemy | Object.Animate_MoveRight, posX, posY - 1));
                            break;

                        default:
                            Debug.Assert(false);
                            break;
                    }
                }
            }

            //
            // second pass - collect elevators and teleports
            //
            for (int tileY = 0; tileY < TileGrid.Height; tileY++)
            {
                for (int tileX = 0; tileX < TileGrid.Width; tileX++)
                {
                    int tileIndex = TileGrid[tileX, tileY];
                    if (tileIndex >= Level.TileCount)
                    {
                        continue; // not an elevator or teleport
                    }

                    Tile tile = Tiles[tileIndex];
                    if (tile.Type != Tile.TileType.Elevator &&
                        tile.Type != Tile.TileType.Teleport)
                    {
                        continue; // not an elevator or teleport
                    }

                    bool disabled = (SwitchGrid[tileX, tileY] != null);

                    objects.Add(new Object(disabled ? Object.Type_DisabledElevator : Object.Type_Elevator, tileX, tileY));

                    tileX++; // skip next tile, as elevators are two tiles wide
                }
            }

            //
            // update teleports
            //
            for (int i = 0; i < objects.Count; i++)
            {
                Object obj = objects[i];

                int tileX = obj.PosX;
                int tileY = obj.PosY;

                char? teleport = TeleportGrid[tileX, tileY];
                if (teleport != null)
                {
                    // look other matching teleport
                    for (int j = 0; j < objects.Count; j++)
                    {
                        Object other = objects[j];
                        if (i != j && TeleportGrid[other.PosX, other.PosY] == teleport)
                        {
                            // found, so set indices
                            obj.Data = j;
                            other.Data = i;
                            break;
                        }
                    }

                    if (obj.Data == 0)
                    {
                        MessageBox.Show($"Teleport '{teleport}' is dangling.\n\n" +
                                        "The teleport will not work correctly within the game.",
                                        "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            //
            // third pass - collect doors
            //
            for (int tileX = 0; tileX < TileGrid.Width; tileX++)
            {
                for (int tileY = 0; tileY < TileGrid.Height; tileY++)
                {
                    int tileIndex = TileGrid[tileX, tileY];
                    if (tileIndex >= Level.TileCount)
                    {
                        continue; // not a door
                    }

                    Tile tile = Tiles[tileIndex];
                    if (tile.Type != Tile.TileType.Door)
                    {
                        continue; // not a door
                    }

                    bool disabled = (SwitchGrid[tileX, tileY] != null);

                    objects.Add(new Object(disabled ? Object.Type_DisabledDoor : Object.Type_Door, tileX, tileY));

                    tileY++; // skip next tile, as doors are two tiles high
                }
            }

            return [.. objects];
        }

        private void PropagateObjects(Object[] objects)
        {
            char teleport = 'A';

            for (int i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];

                // update level data
                if (obj.Type == Object.Type_Unknown ||obj.Type == Object.Type_Door)
                {
                    continue;
                }

                int tileGridX = obj.PosX;
                int tileGridY = obj.PosY;

                if (obj.Type == Object.Type_Elevator)
                {
                    if (obj.Data != 0)
                    {
                        // populate teleport grid
                        var other = objects[obj.Data];
                        if (other.Type == Object.Type_Elevator && other.Data == i)
                        {
                            TeleportGrid[other.PosX, other.PosY] = teleport;
                            other.Data = 0;
                        }

                        TeleportGrid[tileGridX, tileGridY] = teleport;
                        teleport++;
                    }

                    continue;
                }

                if ((obj.Type & Object.TypeMask) == Object.Type_Enemy)
                {
                    tileGridY++; // adjust as enemies are two tiles high
                } 

                int objectIndex = 0;
                switch (obj.Type)
                {
                    case Object.Type_K9 | Object.Animate_LookLeft:
                        objectIndex = TileGrid.K9LookLeft;
                        break;

                    case Object.Type_K9 | Object.Animate_LookRight:
                        objectIndex = TileGrid.K9LookRight;
                        break;

                    case Object.Type_Enemy | Object.Animate_LookLeft:
                        objectIndex = TileGrid.EnemyLookLeft;
                        break;

                    case Object.Type_Enemy | Object.Animate_LookRight:
                        objectIndex = TileGrid.EnemyLookRight;
                        break;

                    case Object.Type_Enemy | Object.Animate_MoveLeft:
                        objectIndex = TileGrid.EnemyMoveLeft;
                        break;

                    case Object.Type_Enemy | Object.Animate_MoveRight:
                        objectIndex = TileGrid.EnemyMoveRight;
                        break;

                    default:
                        Debug.Assert(false);
                        break;
                }

                TileGrid[tileGridX, tileGridY] = (byte)objectIndex;
            }
        }

        internal bool Validate()
        {
            if (!Palette.Validate())
            {
                return false;
            }

            if (!TileGrid.Validate())
            {
                return false;
            }

            foreach (Tile tile in Tiles)
            {
                if (!tile.Validate())
                {
                    return false;
                }
            }

            return true;
        }

        internal static int GetObjectsCount(Object[] objects, int objectType)
        {
            int count = 0;

            for (int i = 1; i < objects.Length; i++)
            {
                if ((objects[i].Type & Object.TypeMask) == objectType)
                {
                    count++;
                }
            }

            return count;
        }

        internal static Level? Load(string filePathName)
        {
            try
            {
                Level levelData = new();

                using (FileStream fs = File.OpenRead(filePathName))
                {
                    levelData.Read(fs);
                }

                levelData.FilePathName = filePathName;

                if (!levelData.Validate())
                {
                    throw new InvalidDataException();
                }

                return levelData;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unexpected error", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        internal static bool Save(string filePathName, Level levelData)
        {
            string tempFileName = "";

            try
            {
                // write to temp file...
                tempFileName = Path.GetTempFileName();
                using (FileStream fs = File.OpenWrite(tempFileName))
                {
                    levelData.Write(fs);
                }

                // commit changes
                if (File.Exists(filePathName))
                {
                    File.Delete(filePathName);
                }

                File.Move(tempFileName, filePathName);

                levelData.FilePathName = filePathName;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unexpected error", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.Delete(tempFileName);
                return false;
            }
        }
    }
}
