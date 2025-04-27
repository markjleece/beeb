// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LevelEditorForm());
        }
    }
}
