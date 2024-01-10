
using System.Collections.Generic;
using UI.TopWindow.Goal;
using UI.TopWindow.Moves;

namespace UI.TopWindow
{
    public interface ITopWindowViewModel
    {
        List<IGoalViewModel> GoalViewModels { get; }
        IMovesViewModel MovesViewModel { get; }
    }
}
