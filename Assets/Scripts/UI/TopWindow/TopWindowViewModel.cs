using System.Collections.Generic;
using Config;
using Managers;
using UI.TopWindow.Goal;
using UI.TopWindow.Moves;

namespace UI.TopWindow
{
    public class TopWindowViewModel : ITopWindowViewModel
    {
        public List<IGoalViewModel> GoalViewModels { get; }
        public IMovesViewModel MovesViewModel { get; private set; }
        private readonly SettingsManager _settingsManager;
        private readonly GridManager _gridManager;
        
        public TopWindowViewModel(SettingsManager settingsManager, GridManager gridManager)
        {
            _settingsManager = settingsManager;
            _gridManager = gridManager;
            
            var activeLevelData = settingsManager.LocalData.GetData<LevelsData>().GetLevelConfig(
                settingsManager.LocalData.GetData<GameData>().GetActiveLevel());

            GoalViewModels = new List<IGoalViewModel>();
            CreateGoalViewModels(activeLevelData.goalValues);
            CreateMovesViewModel(activeLevelData.MovesCount, gridManager);
        }

        private void CreateGoalViewModels(List<GoalValue> goalValues)
        {
            foreach (var goal in goalValues)
            {
                GoalViewModels.Add(new GoalViewModel(_settingsManager, _gridManager, goal));
            }
        }

        private void CreateMovesViewModel(int moveCount, GridManager gridManager) => 
            MovesViewModel = new MovesViewModel(moveCount, gridManager);
    }
}
