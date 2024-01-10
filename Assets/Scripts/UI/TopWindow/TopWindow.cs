using System.Collections.Generic;
using System.Linq;
using Blocks.Enum;
using UI.TopWindow.Goal;
using UI.TopWindow.Moves;
using UnityEngine;

namespace UI.TopWindow
{
    public class TopWindow : UIEntity
    {
        [SerializeField] private MovesView movesViewInstance;
        [SerializeField] private GoalView goalViewPrefab;
        [SerializeField] private RectTransform goalsContainer;
        
        private readonly List<GoalView> _goalViewInstances = new();
        private ITopWindowViewModel _viewModel;
        
        public void Init(ITopWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            movesViewInstance.Init(viewModel.MovesViewModel);
            CreateInitialGoalViews();
        }

        private void CreateInitialGoalViews()
        {
            var goalViewModels = _viewModel.GoalViewModels;
            for (var i = 0; i < goalViewModels.Count; i++)
            {
                var hasViewInstanceForThisIndex = _goalViewInstances.Count > i;
                var viewModel = goalViewModels[i];
                var view = hasViewInstanceForThisIndex ? _goalViewInstances[i] : Instantiate(goalViewPrefab, goalsContainer);
                view.gameObject.SetActive(true);
                view.Init(viewModel);
                
                if (!hasViewInstanceForThisIndex)
                {
                    _goalViewInstances.Add(view);
                }
            }
        }

        public Vector3 GetGoalPosition(BlockId blockId)
        {
            foreach (var viewModel in _viewModel.GoalViewModels.Where(viewModel => viewModel.GoalId == blockId))
            {
                return viewModel.GetGoalImagePosition;
            }

            return Vector3.zero;
        }
    }
}
