using Commands.Enum;

namespace Commands.Base
{
    public abstract class Command
    {
        public abstract GameCommandType CommandType();
        
        public abstract void Start();
        
        public abstract void Execute();
        
        //when it is fully completed and can be disposed
        public abstract bool IsCompleted();
        
        public abstract bool IsSimultaneous();
    }
}