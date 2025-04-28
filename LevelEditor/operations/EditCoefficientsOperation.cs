// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using LevelEditor.ux;

namespace LevelEditor
{
    class EditCoefficientsOperation : Operation
    {
        internal EditCoefficientsOperation(Level levelData)
        {
            LevelData = levelData;
            OldCoefficients = LevelData.Coefficients.Clone();
            NewCoefficients = LevelData.Coefficients.Clone();
        }

        internal override bool Execute()
        {
            EditCoefficientsDialog dialog = new EditCoefficientsDialog(NewCoefficients);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Redo();
                return true;
            }

            return false;
        }

        internal override void Redo()
        {
            LevelData.Coefficients = NewCoefficients;
        }

        internal override void Undo()
        {
            LevelData.Coefficients = OldCoefficients;
        }

        Level LevelData;
        Coefficients OldCoefficients;
        Coefficients NewCoefficients;
    }
}
