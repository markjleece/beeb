// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor
{
    abstract class Operation
    {
        internal abstract bool Execute();
        internal abstract void Undo();
        internal abstract void Redo();
    }
}
