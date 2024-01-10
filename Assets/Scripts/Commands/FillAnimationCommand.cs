using System;
using Commands.Base;
using Commands.Enum;
using Config;
using DG.Tweening;
using UnityEngine;

namespace Commands
{
    public class FillAnimationCommand : Command
    {
        private const GameCommandType Type = GameCommandType.Fill;
        private bool _isCompleted;
        private readonly bool _isSimultaneous;
        private readonly Coordinate _fromCoordinate;
        private readonly Coordinate _destinationCoordinate;
        private readonly RectTransform _animateTransform;
        private readonly Action _onStart;
        private readonly Action _onComplete;

        public FillAnimationCommand(Coordinate from, Coordinate destination, RectTransform animateTransform,
            bool isSimultaneous = false,
            Action onStart = null, Action onComplete = null)
        {
            _fromCoordinate = from;
            _destinationCoordinate = destination;
            _animateTransform = animateTransform;
            _onStart = onStart;
            _onComplete = onComplete;
            _isSimultaneous = isSimultaneous;
        }

        public override GameCommandType CommandType()
        {
            return Type;
        }

        public override void Start()
        {
            _animateTransform.anchoredPosition = _fromCoordinate.relativePosition;
            
            DOTween.Kill(_animateTransform.anchoredPosition);
            _animateTransform.DOAnchorPos(_destinationCoordinate.relativePosition, GameData.BaseFillAnimationDuration)
                            .SetEase(Ease.OutBounce,1f)
                            .OnStart(()=> _onStart?.Invoke())
                            .OnComplete(() => { _onComplete?.Invoke(); _isCompleted = true; });
        }

        public override void Execute()
        {
            
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
