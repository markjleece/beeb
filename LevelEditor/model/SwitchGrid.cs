// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
                SwitchPairings = new Dictionary<Tuple<int, int>, char>(SwitchPairings)
            };
        }

        internal char? this[int x, int y]
        {
            get
            {
                if (SwitchPairings.TryGetValue(Tuple.Create(x, y), out char switchChar))
                {
                    return switchChar;
                }

                return null;
            }

            set
            {
                var key = Tuple.Create(x, y);

                SwitchPairings.Remove(key);

                if (value != null)
                {
                    SwitchPairings.Add(key, (char)value);
                }
            }
        }

        internal void Read(FileStream fs, Object[] objects, Tile[] tiles, TileGrid tileGrid)
        {
            // read switch pairings
            byte[] buffer = new byte[SerializedSize];
            fs.ReadExactly(buffer);

            // find tile index of switch-off tile
            int switchOffTileIndex = GetSwitchOffTileIndex(tiles);
            if (switchOffTileIndex == -1)
            {
                return;
            }

            // for each set of switch pairings...
            int bufferIndex = 0;
            int pairingIndex = 0;

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

                        if (pairingIndex == switchOffTileCount++)
                        {
                            this[tileX, tileY] = (char)('1' + pairingIndex);
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

                    this[tileX, tileY] = (char)('1' + pairingIndex);
                }

                // populate paired tiles
                int tileListCount = buffer[bufferIndex++];
                for (int j = 0; j < tileListCount; j++)
                {
                    int tileX = buffer[bufferIndex++];
                    int tileY = buffer[bufferIndex++];

                    this[tileX, tileY] = (char)('1' + pairingIndex);
                }

                pairingIndex++;
            }
        }

        internal void Write(FileStream fs, Object[] objects, Tile[] tiles, TileGrid tileGrid)
        {
            List<byte> buffer = [];

            // populate pairings
            List<char> pairings = [];
            Dictionary<char, List<int>> pairedObjects = [];
            Dictionary<char, List<Point>> pairedTiles = [];

            int switchOffTileIndex = GetSwitchOffTileIndex(tiles);

            for (int tileY = 0; tileY < tileGrid.Height; tileY++)
            {
                for (int tileX = 0; tileX < tileGrid.Width; tileX++)
                {
                    if (tileGrid[tileX, tileY] == switchOffTileIndex)
                    {
                        char? pairing = this[tileX, tileY];
                        pairing ??= NextPairing();

                        pairings.Add((char)pairing);
                        pairedObjects.Add((char)pairing, []);
                        pairedTiles.Add((char)pairing, []);
                    }
                }
            }

            // populate pairing tile and object lists
            foreach (var entry in SwitchPairings)
            {
                char pairing = entry.Value;

                int tileX = entry.Key.Item1;
                int tileY = entry.Key.Item2;

                bool paired = false;

                for (int j = 0; j < objects.Length; j++)
                {
                    Object obj = objects[j];
                    if (tileX == obj.PosX && (tileY == obj.PosY || tileY - 1 == obj.PosY))
                    {
                        pairedObjects[pairing].Add(j);
                        paired = true;
                        break;
                    }
                }

                if (!paired)
                {
                    int tileIndex = tileGrid[tileX, tileY];
                    if (tileIndex < Level.TileCount && tiles[tileIndex].Type != Tile.TileType.SwitchOff)
                    {
                        pairedTiles[pairing].Add(new Point(tileX, tileY));
                    }
                }
            }

            // write switch pairings
            foreach (char pairing in pairings)
            {
                // construct switch pairings data
                List<int> switchData = [0];

                List<int>? objectList = pairedObjects[pairing];
                switchData.Add(objectList.Count);
                foreach (var obj in objectList)
                {
                    switchData.Add(obj);
                }

                List<Point>? tileList = pairedTiles[pairing];
                switchData.Add(tileList.Count);
                foreach (var point in tileList)
                {
                    switchData.Add(point.X);
                    switchData.Add(point.Y);
                }

                switchData[0] = switchData.Count; // populate count

                if ((buffer.Count + switchData.Count + 1/*terminator*/) > SerializedSize)
                {
                    MessageBox.Show("Too many switch pairings\n\n" + 
                                    $"Pairings for switch '{pairing}' were not saved.",
                                    "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }

                // buffer.AddRange(switchData)
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

        private static int GetSwitchOffTileIndex(Tile[] tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].Type == Tile.TileType.SwitchOff)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void RemovePairing(int tileX, int tileY, Tile[] tiles, TileGrid tileGrid)
        {
            var key = Tuple.Create(tileX, tileY);
            if (SwitchPairings.TryGetValue(key, out char pairing))
            {
                int? tileIndex = tileGrid[tileX, tileY];

                bool removeAllPairings =
                    (CountPairings((char)pairing) == 2) ||
                    (tileIndex != null &&
                     tileIndex < Level.TileCount &&
                     tiles[(int)tileIndex].Type == Tile.TileType.SwitchOff);

                if (removeAllPairings)
                {
                    RemovePairing(pairing);
                }
                else
                {
                    SwitchPairings.Remove(key);
                }
            }
        }

        private void RemovePairing(char pairing)
        {
            List<Tuple<int, int>> keysToRemove = [];

            foreach (var entry in SwitchPairings)
            {
                if (entry.Value == pairing)
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                SwitchPairings.Remove(key);
            }
        }

        internal int CountPairings(char pairing)
        {
            int pairingCount = 0;
            foreach (var entry in SwitchPairings)
            {
                if (entry.Value == pairing)
                {
                    pairingCount++;
                }
            }
            return pairingCount;
        }

        internal List<char> Pairings()
        {
            List<char> pairings = [];
            foreach (var entry in SwitchPairings)
            {
                if (!pairings.Contains(entry.Value))
                {
                    pairings.Add(entry.Value);
                }
            }

            pairings.Sort();

            return pairings;
        }

        internal char NextPairing()
        {
            var pairings = Pairings();
            char nextPairing = '1';
            while (pairings.Contains(nextPairing))
            {
                nextPairing++;
            }
            return nextPairing;
        }

        private Dictionary<Tuple<int, int>, char> SwitchPairings = [];
    }
}
