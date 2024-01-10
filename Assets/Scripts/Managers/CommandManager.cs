using System.Collections.Generic;
using Commands.Base;
using Commands.Enum;
using Logger;
using Managers.Base;
using Managers.Interface;
using UnityEngine;

namespace Managers
{
    public class CommandManager : ManagerBase, IManagerUpdate
    {
        private readonly Queue<Command> _commandsQueue = new();
        private readonly List<Command> _currentSimultaneousCommands = new();
        private Command _currentCommand;

        public override void Init()
        {
        }

        public override void LateStart()
        {
        }
        
        void IManagerUpdate.UpdateManager()
        {
            CommandUpdate();
            SimultaneousCommandUpdate();
        }

        public void QueueCommand(Command newCommand)
        {
            if (newCommand.IsSimultaneous())
            {
                _currentSimultaneousCommands.Add(newCommand);
                newCommand.Start();
                return;
            }
            
            _commandsQueue.Enqueue(newCommand);
        }
        
        private void CommandUpdate()
        {
            //pick command
            if (_commandsQueue.Count > 0 && _currentCommand == null && _currentSimultaneousCommands.Count <= 0)
            {
                _currentCommand = _commandsQueue.Dequeue();
                _currentCommand.Start();
            }

            if (_currentCommand == null)
            {
                return;
            }
            
            //execute command
            _currentCommand.Execute();
            
            if (_currentCommand != null && _currentCommand.IsCompleted())
            {
                _currentCommand = null;
            }
        }
        
        private void SimultaneousCommandUpdate()
        {
            if (_currentSimultaneousCommands.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < _currentSimultaneousCommands.Count; i++)
            {
                _currentSimultaneousCommands[i].Execute();

                if (_currentSimultaneousCommands[i].IsCompleted())
                {
                    _currentSimultaneousCommands.Remove(_currentSimultaneousCommands[i]);
                }
            }
        }

        //if required in future maybe this can be filtered based on "GameCommandType.cs"
        public bool HasActiveCommand()
        {
            return _currentSimultaneousCommands.Count > 0 || _currentCommand != null;
        }
    }
}