using System.Collections.Generic;
using Blocks.Enum;
using Blocks.View;
using DataManagement;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "NewBlocksData", menuName = "ScriptableObjects/Lists/BlocksData")]
    public class BlocksData : LocalData
    {
        [SerializeField] private BlockView blockPrefab;
        
        [SerializeField] private List<BlockConfig> allBlocks = new();

        public BlockView GetBlockPrefab()
        {
            return blockPrefab;
        }
        
        public List<BlockConfig> GetAllBlockConfigs()
        {
            return new List<BlockConfig>(allBlocks);
        }
        
        public BlockConfig GetBlockConfig(BlockId id)
        {
            return allBlocks.Find(block => block.BlockType == id);
        }
        
        public Sprite GetBlockSprite(BlockId id)
        {
            var block = allBlocks.Find(block => block.BlockType == id);
            return block.BlockSprite;
        }
    }
}
