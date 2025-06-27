// This file is Copyright © 2025 - Mark John Leece - All rights reserved
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
