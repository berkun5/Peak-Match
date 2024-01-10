using Config;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FloatingText
{
    public class FloatingBlockPanel : UIEntity
    {
        [SerializeField] private RectTransform panelRect; 
        [SerializeField] private Image blockImage;
        
        private IFloatingBlockPanelViewModel _viewModel;
        private Tween _floatingTween;
        
        private void OnDisable() => _floatingTween?.Kill();
        
        public void Init(IFloatingBlockPanelViewModel viewModel)
        {
            _viewModel = viewModel;
            blockImage.sprite = viewModel.UpdatedBlockSprite;
            blockImage.rectTransform.sizeDelta = viewModel.Size;
            panelRect.anchoredPosition = viewModel.SpawnPosition;
            panelRect.localScale = viewModel.Scale;
            _floatingTween = AnimateFloating(); 
            _floatingTween.OnComplete(viewModel.CompleteAnimation);
        }
        
        private Tween AnimateFloating()
        {
            const float sequenceInDuration = GameData.FloatingPanelAnimationDuration;
            return panelRect.DOMove(_viewModel.TargetPosition, sequenceInDuration)
                .SetEase(Ease.OutSine);
        }
    }
}