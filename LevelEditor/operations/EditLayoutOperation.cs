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
            SelectLayoutDialog dialog = new(LevelData.TileGrid.Width, LevelData.TileGrid.Height);
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

        readonly Level LevelData;
        readonly TileGrid OldTileGrid;
        readonly TileGrid NewTileGrid;
    }
}
