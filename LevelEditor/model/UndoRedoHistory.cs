// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    internal class UndoRedoHistory
    {
        internal bool Execute(Operation op)
        {
            bool success = op.Execute();
            if (success)
            {
                RedoHistory.Clear();
                UndoHistory.Push(op);
            }

            return success;
        }

        internal bool CanUndo()
        {
            return UndoHistory.Count > 0;
        }

        internal bool CanRedo()
        {
            return RedoHistory.Count > 0;
        }

        internal void Undo()
        {
            Operation op = UndoHistory.Pop();
            op.Undo();
            RedoHistory.Push(op);
        }

        internal void Redo()
        {
            Operation op = RedoHistory.Pop();
            op.Redo();
            UndoHistory.Push(op);
        }

        internal bool HasUnsavedChanges()
        {
            return !(UndoHistory.Count == 0 ||
                     UndoHistory.Peek() is NewLevelOperation ||
                     UndoHistory.Peek() is OpenLevelOperation ||
                     UndoHistory.Peek() is SaveLevelOperation ||
                     UndoHistory.Peek() is SaveAsLevelOperation);
        }

        private Stack<Operation> UndoHistory = new Stack<Operation>();
        private Stack<Operation> RedoHistory = new Stack<Operation>();
    }
}
