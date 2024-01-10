using System;
using TMPro;
using UnityEngine;

namespace UI.TopWindow.Moves
{
    public class MovesView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moveCountText;
        private IMovesViewModel _viewModel;

        private void OnDisable()
        {
            _viewModel?.TotalMove.Unsubscribe(OnMoveCountChanged);
        }

        public void Init(IMovesViewModel viewModel)
        {
            _viewModel = viewModel;
            viewModel?.TotalMove.Subscribe(OnMoveCountChanged);
        }
        
        private void OnMoveCountChanged(string moveCount)
        {
            moveCountText.text = moveCount;
        }
    }
}
