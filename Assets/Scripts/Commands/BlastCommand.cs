using System;
using Blocks.Enum;
using Commands.Base;
using Commands.Enum;
using Config;
using UnityEngine;

namespace Commands
{
    public class BlastCommand : Command
    {
        private const GameCommandType Type = GameCommandType.Blast;
        private bool _isCompleted;
        private readonly bool _isSimultaneous;
        private readonly Coordinate _blastCandidate;
        
        private readonly Action _onStart;
        private readonly Action _onComplete;
        private float _delayTime;
        
        public BlastCommand(Coordinate blastCandidate, bool isSimultaneous = true, float setDelay = 0,Action onStart = null, Action onComplete = null)
        {
            _blastCandidate = blastCandidate;
            _onStart = onStart;
            _onComplete = onComplete;
            _isSimultaneous = isSimultaneous;
            _delayTime = setDelay;
        }

        public override GameCommandType CommandType()
        {
            return Type;
        }

        public override void Start()
        {
            _delayTime += Time.time;
            _blastCandidate.startBlockType = BlockId.None;
            _onStart?.Invoke();
        }

        public override void Execute()
        {
            if (Time.time < _delayTime)
            {
                return;
            }
            
            _isCompleted = true;
            _onComplete?.Invoke();
        }

        public override bool IsCompleted()
        {
            return _isCompleted;
        }

        public override bool IsSimultaneous()
        {
            return _isSimultaneous;
        }
    }
}
