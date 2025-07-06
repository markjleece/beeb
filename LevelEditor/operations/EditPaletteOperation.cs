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
            EditPaletteDialog dialog = new(NewPalette);
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

        readonly Palette NewPalette;
        readonly Palette OldPalette;
        readonly Level LevelData;
    }
}
