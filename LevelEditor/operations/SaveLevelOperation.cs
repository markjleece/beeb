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
    class SaveLevelOperation : Operation
    {
        internal SaveLevelOperation(Level currentState)
        {
            CurrentState = currentState;
            OldFilePathName = currentState.FilePathName;
            NewFilePathName = string.Empty;
        }

        internal override bool Execute()
        {
            NewFilePathName = CurrentState.FilePathName;

            if (NewFilePathName == string.Empty)
            {
                SaveFileDialog saveAsFileDialog = new()
                {
                    InitialDirectory = Path.GetDirectoryName(AppSettings.Instance.RecentFilePathName),
                    Filter = "All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };

                if (saveAsFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                AppSettings.Instance.RecentFilePathName = saveAsFileDialog.FileName;

                NewFilePathName = saveAsFileDialog.FileName;
            }

            if (!Level.Save(NewFilePathName, CurrentState))
            {
                return false;
            }

            Redo();
            return true;
        }

        internal override void Redo()
        {
            CurrentState.FilePathName = NewFilePathName;
        }

        internal override void Undo()
        {
            CurrentState.FilePathName = OldFilePathName;
        }

        private readonly Level CurrentState;
        private readonly string OldFilePathName;
        private string NewFilePathName;
    }
}
