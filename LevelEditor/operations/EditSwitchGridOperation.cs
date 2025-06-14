// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Windows.Forms;

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
