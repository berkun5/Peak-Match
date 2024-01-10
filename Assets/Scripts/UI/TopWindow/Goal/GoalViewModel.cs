using System;
using Blocks.Enum;
using Config;
using Managers;
using ReactiveProperty;
using UnityEngine;

namespace UI.TopWindow.Goal
{
    public class GoalViewModel : IGoalViewModel, IDisposable
    {
        public Vector3 GetGoalImagePosition => SetGoalImageRect.position;
        public RectTransform SetGoalImageRect { get; set; }
        public BlockId GoalId { get; }
        public Sprite GoalSprite { get; }
        public IReactiveProperty<string> GoalRequirement => _goalRequirement;

        private readonly ReactiveProperty<string> _goalRequirement = new();
        private readonly GridManager _gridManager;

        public GoalViewModel(SettingsManager settingsManager, GridManager gridManager, GoalValue goalValue)
        {
            GoalId = goalValue.GoalId;
            _gridManager = gridManager;
            GoalSprite = settingsManager.LocalData.GetData<BlocksData>().GetBlockSprite(goalValue.GoalId);
           _goalRequirement.Value = goalValue.Requirement.ToString();
           _gridManager.GoalValueChanged += OnGoalValueChanged;
        }

        void IDisposable.Dispose()
        {
            _gridManager.GoalValueChanged -= OnGoalValueChanged;
        }
        
        private void OnGoalValueChanged(BlockId blastedBlockId, int remainingValue)
        {
            if (blastedBlockId != GoalId)
            {
                return;
            }
            
            _goalRequirement.Value = remainingValue.ToString();
        }
    }
}
