using Blocks.Enum;
using ReactiveProperty;
using UnityEngine;

namespace UI.TopWindow.Goal
{
    public interface IGoalViewModel
    {
        Vector3 GetGoalImagePosition { get; }
        RectTransform SetGoalImageRect { get; set; }
        BlockId GoalId { get; }
        Sprite GoalSprite { get; }
        IReactiveProperty<string> GoalRequirement { get; }
    }
}
