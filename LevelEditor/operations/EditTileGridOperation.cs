// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    class EditTileGridOperation : Operation
    {
        internal EditTileGridOperation(Level levelData, Point[] tileCoords, int tileIndex)
        {
            LevelData = levelData;
            NewTileIndex = (byte)tileIndex;

            if (NewTileIndex < Level.TileCount)
            {
                Tile.TileType tileType = LevelData.Tiles[NewTileIndex].Type;

                if (tileType == Tile.TileType.Teleport)
                {
                    NewTeleportIndex = LevelData.TeleportGrid.NextIndex();
                }
                else
                {
                    NewTeleportIndex = null;
                }
            }

            TileCoords = new Point[tileCoords.Length];
            OldTileIndices = new byte[tileCoords.Length];
            OldTeleportIndices = new char?[tileCoords.Length];

            for (int i = 0; i < tileCoords.Length; i++)
            {
                TileCoords[i] = tileCoords[i];
                OldTileIndices[i] = levelData.TileGrid[tileCoords[i].X, tileCoords[i].Y];
                OldTeleportIndices[i] = levelData.TeleportGrid[tileCoords[i].X, tileCoords[i].Y];
            }

            // determine if operation modifies the switch grid
            SwitchGridModified = false;
            for (int i = 0; i < tileCoords.Length; i++)
            {
                if (LevelData.SwitchGrid[tileCoords[i].X, tileCoords[i].Y] != null)
                {
                    SwitchGridModified = true;
                    break;
                }
            }

            if (SwitchGridModified)
            {
                NewSwitchGrid = LevelData.SwitchGrid.Clone();
                OldSwitchGrid = LevelData.SwitchGrid.Clone();

                for (int i = 0; i < tileCoords.Length; i++)
                {
                    NewSwitchGrid.RemovePairing(tileCoords[i].X, tileCoords[i].Y, LevelData.Tiles, LevelData.TileGrid);
                }
            }
            else
            {
                NewSwitchGrid = LevelData.SwitchGrid;
                OldSwitchGrid = LevelData.SwitchGrid;
            }
        }

        internal override bool Execute()
        {
            // ensure its not a switch-on tile 
            if (NewTileIndex == LevelData.GetTileIndex(Tile.TileType.SwitchOn))
            {
                MessageBox.Show(
                    "Switches can only be turned on in the game\n\n" + 
                    "Did you mean to place a SwitchOff tile instead?",
                    "Set Tile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (NewTileIndex >= Level.TileCount) // object tile?
            {
                // classify tile index
                bool isK9 = false;
                bool isEnemy = false;

                switch (NewTileIndex)
                {
                    case TileGrid.K9LookLeft:
                    case TileGrid.K9LookRight:
                        isK9 = true;
                        break;

                    case TileGrid.EnemyLookLeft:
                    case TileGrid.EnemyLookRight:
                    case TileGrid.EnemyMoveLeft:
                    case TileGrid.EnemyMoveRight:
                        isEnemy = true;
                        break;
                }

                // check if the maximum number of objects has been reached
                Object[] objects = LevelData.CollectObjects();

                if (isK9 && objects[0].Type != Object.Type_Unknown)
                {
                    MessageBox.Show(
                        "K9 has already been placed!",
                        "Set Character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (isEnemy)
                {
                    int count = Level.GetObjectsCount(objects, Object.Type_Enemy);
                    if (count >= Object.MaxEnemyCount)
                    {
                        MessageBox.Show(
                            "The maximum number of enemies have been placed!",
                            "Set Character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (objects.Length >= Level.ObjectCount)
                    {
                        MessageBox.Show(
                            "The maximum number of objects (enemies + elevators + doors) have been placed!",
                            "Set Character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // check that the character is placed in space
                for (int i = 0; i < (isEnemy ? 4 : 2); i++)
                {
                    Point coords = new(TileCoords[0].X + i % 2, TileCoords[0].Y - i / 2);
                    if (coords.X >= LevelData.TileGrid.Width || coords.Y < 0)
                    {
                        MessageBox.Show(
                            "Characters cannot cross tile grid boundaries.\n\n" +
                            "Ensure there is sufficient space above and to the right for the character.", 
                            "Set Character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    int tileIndex = LevelData.TileGrid[coords.X, coords.Y];
                    if (tileIndex >= Level.TileCount || LevelData.Tiles[tileIndex].Type != Tile.TileType.Space)
                    {
                        MessageBox.Show(
                            "Characters must be placed in space.\n\n" +
                            "Ensure there is also sufficient space above and to the right for the character.",
                            "Set Character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            Redo();
    
            return true;
        }

        internal override void Redo()
        {
            for (int i = 0; i < TileCoords.Length; i++)
            {
                LevelData.TileGrid[TileCoords[i].X, TileCoords[i].Y] = NewTileIndex;
                LevelData.TeleportGrid[TileCoords[i].X, TileCoords[i].Y] = NewTeleportIndex;
                LevelData.SwitchGrid = NewSwitchGrid;
            }
        }

        internal override void Undo()
        {
            for (int i = 0; i < TileCoords.Length; i++)
            {
                LevelData.TileGrid[TileCoords[i].X, TileCoords[i].Y] = OldTileIndices[i];
                LevelData.TeleportGrid[TileCoords[i].X, TileCoords[i].Y] = OldTeleportIndices[i];
                LevelData.SwitchGrid = OldSwitchGrid;
            }
        }

        private readonly Point[] TileCoords;
        private readonly byte[] OldTileIndices;
        private readonly char?[] OldTeleportIndices;
        private readonly byte NewTileIndex;
        private readonly char? NewTeleportIndex;
        private readonly SwitchGrid OldSwitchGrid;
        private readonly SwitchGrid NewSwitchGrid;
        private readonly Level LevelData;
        internal readonly bool SwitchGridModified;
    }
}
