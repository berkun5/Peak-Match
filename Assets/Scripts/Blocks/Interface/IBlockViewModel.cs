using Config;
using ReactiveProperty;
using UnityEngine;

namespace Blocks.Interface
{
    public interface IBlockViewModel
    {
        Vector2 BlockSize { get; }
        Vector2 BlockRectPosition { get; }
        RectTransform GetAnimateTransform();
        Coordinate GetCoordinate();
        IReactiveProperty<Sprite> BlockSprite { get; }
        IReactiveProperty<bool> TriggerParticles { get; }
        void SetAnimateTransform(RectTransform animateTransform);
        void HandleBlockPress();
    }
}
