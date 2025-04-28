// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using LevelEditor.ux;

namespace LevelEditor
{
    class EditPaletteOperation : Operation
    {
        internal EditPaletteOperation(Level levelData)
        {
            LevelData = levelData;
            OldPalette = levelData.Palette.Clone();
            NewPalette = levelData.Palette.Clone();
        }

        internal override bool Execute()
        {
            EditPaletteDialog dialog = new EditPaletteDialog(NewPalette);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Redo();
                return true;
            }

            return false;
        }

        internal override void Redo()
        {
            LevelData.Palette = NewPalette;
        }

        internal override void Undo()
        {
            LevelData.Palette = OldPalette;
        }

        Palette NewPalette;
        Palette OldPalette;
        Level LevelData;
    }
}
