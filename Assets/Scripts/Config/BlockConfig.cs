using Blocks.Enum;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "NewBlockConfig", menuName = "ScriptableObjects/BlockConfig")]
    public class BlockConfig : ScriptableObject
    {
        public BlockId BlockType => blockType;
        public Sprite BlockSprite => blockSprite;
        
        [SerializeField] private BlockId blockType;
        [SerializeField] private Sprite blockSprite;
    }
}