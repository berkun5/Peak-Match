using System.Collections.Generic;
using Blocks.Enum;
using Blocks.Interface;
using Commands;
using Config;
using Managers;
using Managers.UI;
using UnityEngine;

namespace Blocks
{
    public class BalloonBlockBehaviour : IBlockBehavior
    {
        public IBlockViewModel BlockViewModel { get; set; }
        public CommandManager CommandManager { get; set; }
        public GridManager GridManager { get; set; }
        public UIEntityManager UIEntityManager { get; set; }
        public GameManager GameManager { get; set; }
        public BlocksData BlocksData { get; set; }

        private ISet<Vector2Int> _neighboursGridPositions = new HashSet<Vector2Int>();
        
        public void Init()
        {
            CacheNeighbours();
        }
        
        public void OnAnimateFill(Coordinate from, Coordinate destination)
        {
            CacheNeighbours();
            CommandManager.QueueCommand(new FillAnimationCommand(from, destination,
                BlockViewModel.GetAnimateTransform(), true));
        }

        public void OnCoordinateBlasted(Vector2Int gridPosition, bool self, BlockId blastedBlockId, bool isGoal, BlastReason blastReason)
        {
            if (self || !_neighboursGridPositions.Contains(gridPosition))
            {
                return;
            }

            if (blastReason == BlastReason.Directional)
            {
                return;
            }
            
            GridManager.SingleBlast(BlockViewModel.GetCoordinate(), true);
        }

        public void OnAnimateFall(Coordinate destination)
        {
            CommandManager.QueueCommand(new FallAnimationCommand(destination,
                BlockViewModel.GetAnimateTransform(), true));
        }

        public void OnBlockPressed(Coordinate pressedBlock)
        {
            
        }

        private void CacheNeighbours()
        {
            _neighboursGridPositions = GridManager.GetAdjacentPositions(BlockViewModel.GetCoordinate().gridPosition);
        }
    }
}
