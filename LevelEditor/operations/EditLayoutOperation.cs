// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using LevelEditor.ux;

namespace LevelEditor
{
    class EditLayoutOperation : Operation
    {
        internal EditLayoutOperation(Level levelData)
        {
            LevelData = levelData;
            OldTileGrid = LevelData.TileGrid.Clone();
            NewTileGrid = LevelData.TileGrid.Clone();
        }

        internal override bool Execute()
        {
            SelectLayoutDialog dialog = new SelectLayoutDialog(LevelData.TileGrid.Width, LevelData.TileGrid.Height);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                NewTileGrid.Resize(dialog.LayoutWidth, dialog.LayoutHeight);
                Redo();
                return true;
            }

            return false;
        }

        internal override void Redo()
        {
            LevelData.TileGrid = NewTileGrid;
        }

        internal override void Undo()
        {
            LevelData.TileGrid = OldTileGrid;
        }

        Level LevelData;
        TileGrid OldTileGrid;
        TileGrid NewTileGrid;
    }
}
