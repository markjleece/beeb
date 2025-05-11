// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    class NewLevelOperation : Operation
    {
        internal NewLevelOperation(Level currentState)
        {
            CurrentState = currentState; 
            OldState = currentState.Clone();
            NewState = new();
        }

        internal override bool Execute()
        {
            Redo();
            return true;
        }

        internal override void Redo()
        {
            CurrentState.Set(NewState);
        }

        internal override void Undo()
        {
            CurrentState.Set(OldState);
        }

        private readonly Level CurrentState;
        private readonly Level NewState;
        private readonly Level OldState;
    }
}
