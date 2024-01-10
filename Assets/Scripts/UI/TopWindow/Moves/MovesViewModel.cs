using System;
using Managers;
using ReactiveProperty;
using UnityEngine;

namespace UI.TopWindow.Moves
{
    public class MovesViewModel : IMovesViewModel, IDisposable
    {
        public IReactiveProperty<string> TotalMove => _totalMove;
        private readonly ReactiveProperty<string> _totalMove = new();
        private readonly GridManager _gridManager;
        
        public MovesViewModel(int moveCount, GridManager gridManager)
        {
            _totalMove.Value = moveCount.ToString();
            _gridManager = gridManager;
            gridManager.MovesValueChanged += OnTotalMovesChanged;
        }

        void IDisposable.Dispose() => _gridManager.MovesValueChanged -= OnTotalMovesChanged;
        
        private void OnTotalMovesChanged(int newMoveCount) => _totalMove.Value = newMoveCount.ToString();
    }
}
