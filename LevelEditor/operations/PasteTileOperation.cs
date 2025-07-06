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
    class PasteTileOperation : Operation
    {
        internal PasteTileOperation(Level levelData, int tileIndex, Tile copiedTile)
        {
            LevelData = levelData;
            TileIndex = tileIndex;
            OldTile = levelData.Tiles[tileIndex].Clone();
            NewTile = copiedTile.Clone();
        }

        internal override bool Execute()
        {
            if (NewTile.Type == Tile.TileType.SwitchOff && IsTileAlreadyDefined(Tile.TileType.SwitchOff))
            {
                MessageBox.Show("The pasted tile is converted to a 'wall'. Only one SwitchOff tile can be defined", "Paste Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConvertToWall = true;
            }

            if (NewTile.Type == Tile.TileType.SwitchOn && IsTileAlreadyDefined(Tile.TileType.SwitchOn))
            {
                MessageBox.Show("The pasted tile is converted to a wall. Only one SwitchOn tile can be defined", "Paste Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConvertToWall = true;
            }

            Redo();
            return true;
        }

        internal override void Redo()
        {
            LevelData.Tiles[TileIndex] = NewTile;
            if (ConvertToWall)
            {
                LevelData.Tiles[TileIndex].Type = Tile.TileType.Wall;
            }
        }

        internal override void Undo()
        {
            LevelData.Tiles[TileIndex] = OldTile;
        }

        private bool IsTileAlreadyDefined(Tile.TileType tileType)
        {
            for (int i = 0; i < LevelData.Tiles.Length; i++)
            {
                if (i != TileIndex && LevelData.Tiles[i].Type == tileType)
                {
                    return true;
                }
            }
            return false;
        }

        private readonly Level LevelData;
        private readonly int TileIndex;
        private readonly Tile NewTile;
        private readonly Tile OldTile;
        private bool ConvertToWall;
    }
}
