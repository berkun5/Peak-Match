using Config;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TopWindow.Goal
{
    public class GoalView : MonoBehaviour
    {
        [SerializeField] private Image goalImage;
        [SerializeField] private TextMeshProUGUI goalText;

        private Vector2 _goalTextInitPos;
        private IGoalViewModel _viewModel;

        private void OnDisable()
        {
            _viewModel?.GoalRequirement.Unsubscribe(OnGoalRequirementChanged);
        }

        public void Init(IGoalViewModel viewModel)
        {
            _viewModel = viewModel;
           viewModel.SetGoalImageRect = goalImage.rectTransform;
            goalImage.sprite = viewModel.GoalSprite;
            _goalTextInitPos = goalText.rectTransform.anchoredPosition;
            
            viewModel.GoalRequirement.Subscribe(OnGoalRequirementChanged);
        }

        private void OnGoalRequirementChanged(string remainingRequirement)
        {
             const int peakYPos = 10;
             
            goalText.rectTransform.DOAnchorPosY(goalText.rectTransform.anchoredPosition.y + peakYPos,0.15f)
                .SetDelay(GameData.FloatingPanelAnimationDuration)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    goalText.text = remainingRequirement;
                    goalText.rectTransform.localScale = Vector3.one;
                    goalText.rectTransform.anchoredPosition = _goalTextInitPos;
                });
        }
    }
}
