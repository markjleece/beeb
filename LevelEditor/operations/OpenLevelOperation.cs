// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor
{
    class OpenLevelOperation : Operation
    {
        internal OpenLevelOperation(Level currentState)
        {
            CurrentState = currentState;
            OldState = currentState.Clone();
            NewState = new Level();
        }

        internal override bool Execute()
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Path.GetDirectoryName(AppSettings.Instance.RecentFilePathName),
                Filter = "Level data files (*.dat)|*.dat",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            AppSettings.Instance.RecentFilePathName = openFileDialog.FileName;

            string filePath = openFileDialog.FileName;
            Level? newLevel = Level.Load(filePath);
            if (newLevel != null)
            {
                NewState = newLevel;
                Redo();
                return true;
            }

            return false;
        }

        internal override void Redo()
        {
            CurrentState.Set(NewState);
        }

        internal override void Undo()
        {
            CurrentState.Set(OldState);
        }

        private Level CurrentState;
        private Level NewState;
        private Level OldState;
    }
}
