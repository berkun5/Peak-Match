using Blocks.Enum;
using Config;
using Managers;
using Managers.UI;
using UnityEngine;

namespace Blocks.Interface
{
    public interface IBlockBehavior
    {
        IBlockViewModel BlockViewModel { set; }
        CommandManager CommandManager { set; }
        GridManager GridManager { set; }
        UIEntityManager UIEntityManager { set; }
        GameManager GameManager { set; }
        BlocksData BlocksData { set; }
        
        void Init();
        void OnAnimateFill(Coordinate from, Coordinate destination);
        void OnCoordinateBlasted(Vector2Int gridPosition, bool self, BlockId blastedBlockId, bool isGoal, BlastReason blastReason);
        void OnAnimateFall(Coordinate destination);
        void OnBlockPressed(Coordinate pressedBlock);
    }
}