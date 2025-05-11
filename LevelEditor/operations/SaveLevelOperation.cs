// This file is Copyright © 2025 - Mark John Leece - All rights reserved
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
