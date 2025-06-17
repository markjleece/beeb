// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Channels;
using System.Windows.Forms;

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

            // populate switch pairings
            Dictionary<int, char> pairings = [];

            int switchOffTileIndex = GetSwitchOffTileIndex(tiles);
            char pairing = '1';

            for (int tileY = 0; tileY < tileGrid.Height; tileY++)
            {
                for (int tileX = 0; tileX < tileGrid.Width; tileX++)
                {
                    if (tileGrid[tileX, tileY] == switchOffTileIndex)
                    {
                        this[tileX, tileY] = pairing;
                        pairings.Add(tileGrid.GetRuntimeAddress(tileX, tileY), pairing++);
                    }
                }
            }

            // for each set of switch pairings...
            int bufferIndex = 0;

            while (bufferIndex < SerializedSize - 2)
            {
                // switch address
                int switchAddress = buffer[bufferIndex++];
                switchAddress += buffer[bufferIndex++] * 256;
                if (switchAddress == 0)
                {
                    break;
                }

                pairing = pairings[switchAddress];

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

                    this[tileX, tileY] = pairing;
                }

                // populate paired tiles
                int tileListCount = buffer[bufferIndex++];
                for (int j = 0; j < tileListCount; j++)
                {
                    // read lever coords
                    int leftLo = buffer[bufferIndex++];
                    int leftHi = buffer[bufferIndex++];
                    int topLo = buffer[bufferIndex++];
                    int topHi = buffer[bufferIndex++];

                    // convert to tile coords
                    int tileX = (256 * leftHi + leftLo) / 16;
                    int tileY = (256 * topHi + topLo) / 16;

                    this[tileX, tileY] = pairing;
                }
            }
        }

        internal void Write(FileStream fs, Object[] objects, Tile[] tiles, TileGrid tileGrid)
        {
            List<byte> buffer = [];

            // collect pairings
            List<char> pairings = [];
            Dictionary<char, int> pairedSwitchAddresses = [];
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
                        pairedSwitchAddresses.Add((char)pairing, tileGrid.GetRuntimeAddress(tileX, tileY));
                        pairedObjects.Add((char)pairing, []);
                        pairedTiles.Add((char)pairing, []);
                    }
                }
            }

            // populate tile and object pairings
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
            int remainingPairingBytes = SerializedSize - (pairings.Count * 4);
            if (remainingPairingBytes < 0)
            {
                throw new Exception("Too many switches to save");
            }

            foreach (char pairing in pairings)
            {
                // construct switch pairings data
                List<int> switchData = [];

                int switchAddress = pairedSwitchAddresses[pairing];
                switchData.Add(switchAddress % 256); // low-byte
                switchData.Add(switchAddress / 256); // high-byte

                List<int> objectList = pairedObjects[pairing];
                List<Point> tileList = pairedTiles[pairing];

                int pairingBytes = objectList.Count + tileList.Count * 4;

                if (pairingBytes <= remainingPairingBytes)
                {
                    remainingPairingBytes -= pairingBytes;

                    switchData.Add(objectList.Count);
                    foreach (var obj in objectList)
                    {
                        switchData.Add(obj);
                    }

                    switchData.Add(tileList.Count);
                    foreach (var point in tileList)
                    {
                        var levelCoords = new Point(16 * point.X, 16 * point.Y);
                        switchData.Add(levelCoords.X % 256);
                        switchData.Add(levelCoords.X / 256);
                        switchData.Add(levelCoords.Y % 256);
                        switchData.Add(levelCoords.Y / 256);
                    }
                }
                else
                {
                    MessageBox.Show("Too many switch pairings\n\n" +
                                    $"Pairings for switch '{pairing}' were not saved.",
                                    "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    switchData.Add(0); // paired object count
                    switchData.Add(0); // paired tile count
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
