using System;
using Blocks.Enum;
using Blocks.Interface;
using Commands;
using Config;
using Managers;
using Managers.UI;
using UI.FloatingText;
using UnityEngine;

namespace Blocks
{
    public class RocketBlockBehaviour : IBlockBehavior
    {
        public IBlockViewModel BlockViewModel { get; set; }
        public CommandManager CommandManager { get; set; }
        public GridManager GridManager { get; set; }
        public UIEntityManager UIEntityManager { get; set; }
        public GameManager GameManager { get; set; }
        public BlocksData BlocksData { get; set; }

        private readonly BlastDirection _blastDirection;

        public RocketBlockBehaviour(bool verticalRocket)
        {
            _blastDirection = verticalRocket ? BlastDirection.Vertical : BlastDirection.Horizontal;
        }
        
        public void Init()
        {
            
        }

        public void OnAnimateFill(Coordinate from, Coordinate destination)
        {
            CommandManager.QueueCommand(new FillAnimationCommand(from, destination,
                BlockViewModel.GetAnimateTransform(), true));
        }

        public void OnCoordinateBlasted(Vector2Int gridPosition, bool self, BlockId blastedBlockId, bool isGoal, BlastReason blastReason)
        {
            self = BlockViewModel.GetCoordinate().gridPosition == gridPosition;

            if (!self)
            {
                return;
            }
            
            var rowCount = GridManager.ActiveLevelConfig.RowCount; 
            var columnCount = GridManager.ActiveLevelConfig.ColumnCount;
            var goalPositions = new Vector3[2];
            var goalScales = new[] {Vector3.one, Vector3.one};
            var gridEndPos = Mathf.Pow(Mathf.Max(rowCount,columnCount) + 1, 2); //power of 2 of greater end of the grid
            var startPos = BlockViewModel.GetAnimateTransform().position;
                
            if (_blastDirection == BlastDirection.Vertical)
            {
                goalPositions[0] = startPos + Vector3.right * gridEndPos;
                goalPositions[1] = startPos + Vector3.left * gridEndPos;
                goalScales[1] = new Vector3(-1, 1, 1);
            }
            else
            {
                goalPositions[0] = startPos + Vector3.up * gridEndPos;
                goalPositions[1] = startPos + Vector3.down * gridEndPos;
                goalScales[1] = new Vector3(1, -1, 1);
            }

            for (var i = 0; i < goalPositions.Length; i++)
            {
                var index = i;
                UIEntityManager.Show<FloatingBlockPanel>(window
                    => window.Init(new FloatingBlockPanelViewModel(BlocksData.GetBlockConfig(blastedBlockId).BlockSprite,
                        BlockViewModel.BlockRectPosition, goalPositions[index], BlockViewModel.BlockSize, goalScales[index])));
            }
            
            //this feature is about the game design I guess,
            //if i keep blasting them recursively while new rockets are falling, then it's unpredictable
            if (blastReason == BlastReason.Directional)
            {
                return;
            }
            GridManager.BlastInDirection(BlockViewModel.GetCoordinate(), _blastDirection,true);
        }

        public void OnAnimateFall(Coordinate destination)
        {
            CommandManager.QueueCommand(new FallAnimationCommand(destination,
                BlockViewModel.GetAnimateTransform(), true));
        }

        public void OnBlockPressed(Coordinate pressedBlock)
        {
            GridManager.BlastInDirection(pressedBlock, _blastDirection);
        }
    }
}
