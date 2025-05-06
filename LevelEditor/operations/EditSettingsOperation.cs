// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using LevelEditor.ux;

namespace LevelEditor
{
    class EditSettingsOperation : Operation
    {
        internal EditSettingsOperation(Level levelData)
        {
            LevelData = levelData;
            OldSettings = LevelData.Settings.Clone();
            NewSettings = LevelData.Settings.Clone();
        }

        internal override bool Execute()
        {
            EditSettingsDialog dialog = new EditSettingsDialog(NewSettings);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Redo();
                return true;
            }

            return false;
        }

        internal override void Redo()
        {
            LevelData.Settings = NewSettings;
        }

        internal override void Undo()
        {
            LevelData.Settings = OldSettings;
        }

        Level LevelData;
        Settings OldSettings;
        Settings NewSettings;
    }
}
