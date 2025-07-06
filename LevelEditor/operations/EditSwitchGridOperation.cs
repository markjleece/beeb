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
    class EditSwitchGridOperation : Operation
    {
        internal EditSwitchGridOperation(Level levelData, int tileX, int tileY)
        {
            LevelData = levelData;
            OldSwitchGrid = levelData.SwitchGrid.Clone();
            NewSwitchGrid = LevelData.SwitchGrid.Clone();
            TileCoords = new Point(tileX, tileY);
        }

        internal override bool Execute()
        {
            // clicked on a switch?
            int? tileIndex = LevelData.TileGrid[TileCoords.X, TileCoords.Y];

            if (tileIndex != null &&
                tileIndex < Level.TileCount &&
                LevelData.Tiles[(int)tileIndex].Type == Tile.TileType.SwitchOff)
            {
                // clicked on a switch
                char? pairing = NewSwitchGrid[TileCoords.X, TileCoords.Y];
                if (pairing != null)
                {
                    CurrentPairing = pairing;
                }
                else
                {
                    CurrentPairing = NewSwitchGrid.NextPairing();
                    NewSwitchGrid[TileCoords.X, TileCoords.Y] = CurrentPairing;
                }
            }
            else if (CurrentPairing == null)
            {
                MessageBox.Show(
                    "You need to control-click on a switch first!",
                    "Set Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                char? pairing = NewSwitchGrid[TileCoords.X, TileCoords.Y];
                if (pairing != CurrentPairing)
                {
                    NewSwitchGrid.RemovePairing(TileCoords.X, TileCoords.Y, LevelData.Tiles, LevelData.TileGrid);
                    NewSwitchGrid[TileCoords.X, TileCoords.Y] = CurrentPairing;
                }
            }

            Redo();
    
            return true;
        }

        internal override void Redo()
        {
            LevelData.SwitchGrid = NewSwitchGrid;
        }

        internal override void Undo()
        {
            LevelData.SwitchGrid = OldSwitchGrid;
        }

        private readonly SwitchGrid OldSwitchGrid;
        private readonly SwitchGrid NewSwitchGrid;
        private readonly Level LevelData;
        private readonly Point TileCoords;

        static internal char? CurrentPairing;
    }
}
