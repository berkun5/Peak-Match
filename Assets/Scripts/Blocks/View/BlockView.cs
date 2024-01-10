using Blocks.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Blocks.View
{
    public class BlockView : MonoBehaviour
    {
        public RectTransform BlockRect => blockRect;
        public Image BlockImage => blockImage;

        [SerializeField] private RectTransform blockRect;
        [SerializeField] private Image blockImage;
        [SerializeField] private Button blockButton;
        [SerializeField] private ParticleSystem blockParticles;
        
        private IBlockViewModel _viewModel;

        private void OnDisable()
        {
            _viewModel?.BlockSprite.Unsubscribe(OnBlockSpriteChanged);
            _viewModel?.TriggerParticles.Unsubscribe(OnParticlesTriggered);
            blockButton.onClick.RemoveListener(OnBlockPressed);
        }

        private void OnEnable()
        {
            _viewModel?.BlockSprite.Subscribe(OnBlockSpriteChanged);
            _viewModel?.TriggerParticles.Subscribe(OnParticlesTriggered);
            blockButton.onClick.AddListener(OnBlockPressed);
        }
        
        public void Init(IBlockViewModel viewModel)
        {
            _viewModel = viewModel;
            viewModel.SetAnimateTransform(blockRect);
            blockRect.sizeDelta = viewModel.BlockSize;
            blockRect.anchoredPosition = viewModel.BlockRectPosition;
            
            viewModel.TriggerParticles.Subscribe(OnParticlesTriggered);
            viewModel.BlockSprite.Subscribe(OnBlockSpriteChanged);
        }
        
        private void OnBlockSpriteChanged(Sprite newSprite)
        {
            blockImage.sprite = newSprite;
        }
        
        private void OnBlockPressed()
        {
            _viewModel.HandleBlockPress();
        }
        
        private void OnParticlesTriggered(bool trigger) => blockParticles.Play();
    }
}
