using System;
using Commands.Base;
using Commands.Enum;
using Config;
using DG.Tweening;
using UnityEngine;

namespace Commands
{
    public class FallAnimationCommand : Command
    {
        private const GameCommandType Type = GameCommandType.Fall;
        private bool _isCompleted;
        private readonly bool _isSimultaneous;
        private readonly Vector2 _fromPosition;
        private readonly Coordinate _destinationCoordinate;
        private readonly RectTransform _animateTransform;
        private readonly Action _onStart;
        private readonly Action _onComplete;
        private readonly float _additionalDurationPerDistance;
        
        public FallAnimationCommand(Coordinate destination, RectTransform animateTransform, bool isSimultaneous = false,
            Action onStart = null, Action onComplete = null)
        {
            _destinationCoordinate = destination;
            _animateTransform = animateTransform;
            _onStart = onStart;
            _onComplete = onComplete;
            _isSimultaneous = isSimultaneous;

            var anchoredPosition = animateTransform.anchoredPosition;
            var yStartPos = anchoredPosition.y + animateTransform.rect.height / 2 + Screen.height;
            _fromPosition = new Vector2(anchoredPosition.x, yStartPos);
        }
        
        public override GameCommandType CommandType()
        {
            return Type;
        }

        public override void Start()
        {
            _animateTransform.anchoredPosition = _fromPosition;
            
            DOTween.Kill(_animateTransform.anchoredPosition);
            _animateTransform.DOAnchorPos(_destinationCoordinate.relativePosition, GameData.BaseFallAnimationDuration)
                            .SetEase(Ease.OutBack, 1f)
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
