#if UNITY_EDITOR
using Blocks.Enum;
using Config;
using UnityEngine;

namespace EditorTools.LevelBuilder.Scripts
{
    public class LevelBuilderBlock : MonoBehaviour
    {
        public RectTransform BlockRect => blockRect;
        
        public BlockId Id { get; set; }
        [SerializeField] private RectTransform blockRect;
        [SerializeField] private BlocksData blocksData;

        private LevelBuilder _levelBuilder;
        private bool _isInitialized;
        
        public void Init(LevelBuilder levelBuilder)
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            _levelBuilder = levelBuilder;
        }
        
        public void OnCoordinatesChanged(Coordinate newCoordinates)
        {
            blockRect.sizeDelta = newCoordinates.relativeSize;
            blockRect.anchoredPosition = newCoordinates.relativePosition;
        }

        public void ChangeId(BlockId blockId)
        {
            Id = blockId;
            if(_levelBuilder)
                _levelBuilder.ChangeBlockType(transform.GetSiblingIndex(), Id);
        }
        
        private void OnDrawGizmos() 
        {
            if (Id == BlockId.None)
            {
                return;
            } 
            var rect = blockRect.rect; 
            var anchoredPosition = blockRect.position; 
            var xPos = anchoredPosition.x - rect.width / 2; 
            var yPos = anchoredPosition.y + rect.height / 2;
            Gizmos.DrawGUITexture(new Rect(xPos, yPos, rect.width, -rect.height), blocksData.GetBlockSprite(Id).texture);
        }
    }
}
#endif