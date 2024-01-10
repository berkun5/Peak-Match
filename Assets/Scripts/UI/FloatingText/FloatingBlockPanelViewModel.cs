using GameServices;
using GameServices.ServiceLocator;
using Managers.UI;
using UnityEngine;

namespace UI.FloatingText
{
    public class FloatingBlockPanelViewModel : IFloatingBlockPanelViewModel
    {
        public Vector3 TargetPosition { get; }
        public Vector3 SpawnPosition { get; }
        public Vector3 Size { get; }
        public Vector3 Scale { get; }
        
        private readonly Sprite _updatedBlockSprite;
        private readonly UIEntityManager _uiEntityManager; 
        Sprite IFloatingBlockPanelViewModel.UpdatedBlockSprite => _updatedBlockSprite;
        
        public FloatingBlockPanelViewModel(Sprite updatedSprite, Vector3 spawnPosition, Vector3 targetPosition,
            Vector2 size, Vector3 scale)
        {
            _uiEntityManager = GameServiceLocator.GetService<UIServiceProvider>().GetManager<UIEntityManager>();
            _updatedBlockSprite = updatedSprite;
            SpawnPosition = spawnPosition;
            TargetPosition = targetPosition;
            Size = size;
            Scale = scale;
        }
        
        void IFloatingBlockPanelViewModel.CompleteAnimation()
        {
            _uiEntityManager.Hide<FloatingBlockPanel>();
        }
        
    }
}
