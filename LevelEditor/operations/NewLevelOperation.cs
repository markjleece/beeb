// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor
{
    class NewLevelOperation : Operation
    {
        internal NewLevelOperation(Level currentState)
        {
            CurrentState = currentState; 
            OldState = currentState.Clone();
            NewState = new Level();
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

        private Level CurrentState;
        private Level NewState;
        private Level OldState;
    }
}
