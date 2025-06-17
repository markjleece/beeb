// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    class EditTileOperation : Operation
    {
        internal EditTileOperation(Level levelData, int tileIndex)
        {
            LevelData = levelData;
            TileIndex = tileIndex;
            OldTile = levelData.Tiles[tileIndex < Level.TileCount ? tileIndex : 0].Clone();
            NewTile = levelData.Tiles[tileIndex < Level.TileCount ? tileIndex : 0].Clone();
        }

        internal override bool Execute()
        {
            if (TileIndex == 0)
            {
                MessageBox.Show("The first tile is always space, filled with the background color. It cannot be edited", "Edit Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (TileIndex >= Level.TileCount)
            {
                MessageBox.Show("This last six tiles are object tiles. They cannot be editied", "Edit Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            EditTileDialog dialog = new(NewTile, LevelData.Palette);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (NewTile.Type == Tile.TileType.SwitchOff && IsTileAlreadyDefined(Tile.TileType.SwitchOff))
                {
                    MessageBox.Show("Only one SwitchOff tile can be defined", "Edit Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (NewTile.Type == Tile.TileType.SwitchOn && IsTileAlreadyDefined(Tile.TileType.SwitchOn))
                {
                    MessageBox.Show("Only one SwitchOn tile can be defined", "Edit Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                Redo();
                return true;
            }

            return false;
        }

        internal override void Redo()
        {
            LevelData.Tiles[TileIndex] = NewTile;
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
    }
}
