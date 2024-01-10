using UnityEngine;

namespace UI.FloatingText
{
    public interface IFloatingBlockPanelViewModel 
    {
        Vector3 TargetPosition { get; }
        Vector3 SpawnPosition { get; }
        Vector3 Size { get; }
        Vector3 Scale { get; }
        Sprite UpdatedBlockSprite { get; }
        void CompleteAnimation();
    }
}
