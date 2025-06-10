// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System;
using System.Diagnostics;
using System.Threading.Channels;

namespace LevelEditor
{
    class SwitchGrid
    {
        static readonly int SerializedSize = 16;

        internal SwitchGrid Clone()
        {
            return new SwitchGrid()
            {
                SwitchIndices = new Dictionary<Tuple<int, int>, char>(SwitchIndices)
            };
        }

        internal char? this[int x, int y]
        {
            get
            {
                if (SwitchIndices.TryGetValue(Tuple.Create(x, y), out char switchChar))
                {
                    return switchChar;
                }

                return null;
            }

            set
            {
                var key = Tuple.Create(x, y);
                
                SwitchIndices.Remove(key);

                if (value != null)
                {
                    SwitchIndices.Add(key, (char)value);
                }
            }
        }

        internal void Read(FileStream fs, Object[] objects, Tile[] tiles, TileGrid tileGrid)
        {
            // read switch pairings
            byte[] buffer = new byte[SerializedSize];
            fs.ReadExactly(buffer);

            // find switch-off tile index
            int switchOffTileIndex = 0;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].Type == Tile.TileType.SwitchOff)
                {
                    switchOffTileIndex = i;
                    break;
                }
            }

            // for each set of switch pairings...
            int bufferIndex = 0;
            int switchIndex = 0;

            while (buffer[bufferIndex++] != 0)
            {
                // populate paired switch
                int switchOffTileCount = 0;
                for (int tileY = 0; tileY < tileGrid.Height; tileY++)
                {
                    for (int tileX = 0; tileX < tileGrid.Width; tileX++)
                    {
                        if (tileGrid[tileX, tileY] != switchOffTileIndex)
                        {
                            continue;
                        }

                        if (switchIndex == switchOffTileCount++)
                        {
                            this[tileX, tileY] = (char)('1' + switchIndex);
                            tileX = tileGrid.Width;
                            tileY = tileGrid.Height;
                        }
                    }
                }

                // populate paired objects
                int objectListCount = buffer[bufferIndex++];
                for (int j = 0; j < objectListCount; j++)
                {
                    int objectIndex = buffer[bufferIndex++];
                    Object obj = objects[objectIndex];

                    int tileX = obj.PosX;
                    int tileY = obj.PosY;

                    if ((obj.Type & Object.TypeMask) == Object.Type_Enemy)
                    {
                        tileY++;
                    }

                    this[tileX, tileY] = (char)('1' + switchIndex);
                }

                // populate paired tiles
                int tileListCount = buffer[bufferIndex++];
                for (int j = 0; j < tileListCount; j++)
                {
                    int tileX = buffer[bufferIndex++];
                    int tileY = buffer[bufferIndex++];

                    this[tileX, tileY] = (char)('1' + switchIndex);
                }

                switchIndex++;
            }
        }

        internal void Write(FileStream fs, Object[] objects, Tile[] tiles, TileGrid tileGrid)
        {
            List<byte> buffer = [];

            // collect switch pairings
            Dictionary<char, List<int>> objectsByIndex = [];
            Dictionary<char, List<Point>> tilesByIndex = [];

            foreach (var entry in SwitchIndices)
            {
                char switchIndex = entry.Value;

                int tileX = entry.Key.Item1;
                int tileY = entry.Key.Item2;

                for (int j = 0; j < objects.Length; j++)
                {
                    Object obj = objects[j];
                    if (tileX == obj.PosX && (tileY == obj.PosY || tileY - 1 == obj.PosY))
                    {
                        if (objectsByIndex.TryGetValue(switchIndex, out List<int>? objectList))
                        {
                            objectList.Add(switchIndex);
                        }
                        else
                        {
                            objectsByIndex.Add(switchIndex, [j]);
                        }

                        continue;
                    }
                }

                int tileIndex = tileGrid[tileX, tileY];
                Tile.TileType tileType = tiles[tileIndex].Type;
                if (tileType == Tile.TileType.SwitchOff || tileType == Tile.TileType.SwitchOn)
                {
                    continue; // ignore switches
                }

                if (tilesByIndex.TryGetValue(switchIndex, out List<Point>? tileList))
                {
                    tileList.Add(new Point(tileX, tileY));
                }
                else
                {
                    tilesByIndex.Add(switchIndex, [new Point(tileX, tileY)]);
                }
            }

            // write switch pairings
            List<char> switchIndices = [];

            for (int tileY = 0; tileY < tileGrid.Height; tileY++)
            {
                for (int tileX = 0; tileX < tileGrid.Width; tileX++)
                {
                    char? index = this[tileX, tileY];
                    if (index != null && !switchIndices.Contains((char)index))
                    {
                        switchIndices.Add((char)index);
                    }
                }
            }

            foreach (char switchIndex in switchIndices)
            {
                List<int> switchData = [];

                if (objectsByIndex.TryGetValue(switchIndex, out List<int>? objectList))
                {
                    switchData.Add(objectList.Count);
                    foreach (var obj in objectList)
                    {
                        switchData.Add(obj);
                    }
                }

                if (tilesByIndex.TryGetValue(switchIndex, out List<Point>? tileList))
                {
                    switchData.Add(tileList.Count);
                    foreach (var point in tileList)
                    {
                        switchData.Add(point.X);
                        switchData.Add(point.Y);
                    }
                }

                int switchDataSize = 1/*size*/ + switchData.Count;

                if ((buffer.Count + switchDataSize + 1/*terminator*/) > SerializedSize)
                {
                    MessageBox.Show("Too many switch pairings\n\n" + 
                                    $"Pairings for switch '{switchIndex}' were not saved.",
                                    "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }

                buffer.Add((byte)switchDataSize);
                for (int i = 0; i < switchData.Count; i++)
                {
                    buffer.Add((byte)switchData[i]);
                }
            }

            while (buffer.Count < SerializedSize)
            {
                buffer.Add(0);
            }

            Debug.Assert(buffer.Count == SerializedSize);

            fs.Write([.. buffer]);
        }

        internal List<char> Indices()
        {
            List<char> indices = [];
            foreach (var entry in SwitchIndices)
            {
                if (!indices.Contains(entry.Value))
                {
                    indices.Add(entry.Value);
                }
            }

            indices.Sort();

            return indices;
        }

        internal char NextIndex()
        {
            var indices = Indices();
            char nextIndex = '1';
            while (indices.Contains(nextIndex))
            {
                nextIndex++;
            }
            return nextIndex;
        }

        private Dictionary<Tuple<int, int>, char> SwitchIndices = [];
    }
}
