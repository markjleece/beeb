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

using System.Threading.Channels;

namespace LevelEditor
{
    class TeleportGrid
    {
        internal TeleportGrid Clone()
        {
            return new TeleportGrid()
            {
                TeleportIndices = new OrderedDictionary<Tuple<int, int>, char>(TeleportIndices)
            };
        }

        internal char? this[int x, int y]
        {
            get
            {
                if (TeleportIndices.ContainsKey(Tuple.Create(x - 1, y)))
                {
                    return null; // ignore call
                }

                if (TeleportIndices.TryGetValue(Tuple.Create(x, y), out char teleport))
                {
                    return teleport;
                }

                return null;
            }

            set
            {
                if (TeleportIndices.ContainsKey(Tuple.Create(x - 1, y)))
                {
                    return; // ignore call
                }

                var key = Tuple.Create(x, y);
                
                TeleportIndices.Remove(key);

                if (value != null)
                {
                    TeleportIndices.Add(key, (char)value);
                }
            }
        }

        internal List<char> Indices()
        {
            List<char> indices = [];
            
            foreach (var entry in TeleportIndices)
            {
                if (indices.IndexOf(entry.Value) == -1)
                {
                    indices.Add(entry.Value);
                }
            }

            indices.Sort();

            return indices;
        }

        internal char NextIndex()
        {
            var counts = new Dictionary<char, int>();
            foreach (var entry in TeleportIndices)
            {
                if (counts.TryGetValue(entry.Value, out _))
                {
                    counts[entry.Value]++;
                }
                else
                {
                    counts.Add(entry.Value, 1);
                }
            }

            char nextIndex = 'A';
            foreach (var entry in counts)
            {
                if (entry.Value == 1)
                {
                    return entry.Key;
                }
                else
                {
                    nextIndex = (char)(entry.Key + 1);
                }
            }
            return nextIndex;
        }

        private OrderedDictionary<Tuple<int, int>, char> TeleportIndices = [];
    }
}
