using Blocks.Enum;
using Blocks.Interface;
using Commands;
using Config;
using Managers;
using Managers.UI;
using UnityEngine;

namespace Blocks
{
    public class ColorBlockBehaviour : IBlockBehavior
    {
        public IBlockViewModel BlockViewModel { get; set; }
        public CommandManager CommandManager { get; set; }
        public GridManager GridManager { get; set; }
        public UIEntityManager UIEntityManager { get; set; }
        public GameManager GameManager { get; set; }
        public BlocksData BlocksData { get; set; }

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
            if (!self)
            {
                return;
            }
            BlockViewModel.TriggerParticles.Value = !BlockViewModel.TriggerParticles.Value;
        }

        public void OnAnimateFall(Coordinate destination)
        {
            CommandManager.QueueCommand(new FallAnimationCommand(destination,
                BlockViewModel.GetAnimateTransform(), true));
        }

        public void OnBlockPressed(Coordinate pressedBlock)
        {
            GridManager.TryMatchBlast(pressedBlock);
        }
    }
}