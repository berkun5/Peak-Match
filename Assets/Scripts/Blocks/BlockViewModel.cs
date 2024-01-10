using System;
using Blocks.Enum;
using Blocks.Interface;
using Config;
using Managers;
using Managers.UI;
using ReactiveProperty;
using UI.FloatingText;
using UnityEngine;

namespace Blocks
{
    public class BlockViewModel<T> : IBlockViewModel, IDisposable where T : IBlockBehavior
    {
        public Vector2 BlockSize { get; }
        public Vector2 BlockRectPosition { get; }
        IReactiveProperty<Sprite> IBlockViewModel.BlockSprite => _blockSprite;
        IReactiveProperty<bool> IBlockViewModel.TriggerParticles => _triggerParticles;
        Coordinate IBlockViewModel.GetCoordinate() => _coordinate;
        
        private Coordinate _coordinate;
        private RectTransform _animateTransform;
        
        private readonly GridManager _gridManager;
        private readonly CommandManager _commandManager;
        private readonly UIEntityManager _uiEntityManager;
        private readonly GameManager _gameManager;
        
        private readonly BlocksData _blocksData;
        private readonly ReactiveProperty<Sprite> _blockSprite = new();
        private readonly ReactiveProperty<bool> _triggerParticles = new();
        
        private T _blockBehavior;

        public BlockViewModel(BlockId blockType, Coordinate coordinate, BlocksData blocksData, T blockBehavior,
            GridManager gridManager, CommandManager commandManager, UIEntityManager uiEntityManager,GameManager gameManager)
        {
            _blocksData = blocksData;
            BlockSize = coordinate.relativeSize;
            BlockRectPosition = coordinate.relativePosition;
            _gridManager = gridManager;
            _commandManager = commandManager;
            _uiEntityManager = uiEntityManager;
            _gameManager = gameManager;
            
            SetCoordinate(coordinate);
            SetBlockSprite(blocksData.GetBlockConfig(blockType).BlockSprite);
            ChangeBehaviour(blockBehavior);
            _gridManager.CoordinateChanged += OnCoordinatesChanged;
            _gridManager.CoordinateFillAnimated += OnAnimateFill;
            _gridManager.CoordinateBlasted += OnCoordinateBlasted;
            _gridManager.CoordinateFallAnimated += OnAnimateFall;
        }

        public void Dispose()
        {
            _gridManager.CoordinateChanged -= OnCoordinatesChanged;
            _gridManager.CoordinateFillAnimated -= OnAnimateFill;
            _gridManager.CoordinateBlasted -= OnCoordinateBlasted;
            _gridManager.CoordinateFallAnimated -= OnAnimateFall;
        }

        void IBlockViewModel.HandleBlockPress() => _blockBehavior.OnBlockPressed(_coordinate);

        void IBlockViewModel.SetAnimateTransform(RectTransform animateTransform) => _animateTransform = animateTransform;
        
        RectTransform IBlockViewModel.GetAnimateTransform()
        {
            return _animateTransform;
        }
        
        private void ChangeBehaviour(IBlockBehavior newBlockBehaviour)
        {
            if (newBlockBehaviour is not T typedBlockBehaviour)
            {
                return;
            }
            
            _blockBehavior = typedBlockBehaviour;
            _blockBehavior.BlockViewModel = this;
            _blockBehavior.CommandManager = _commandManager;
            _blockBehavior.GridManager = _gridManager;
            _blockBehavior.UIEntityManager = _uiEntityManager;
            _blockBehavior.GameManager = _gameManager;
            _blockBehavior.BlocksData = _blocksData;
            _blockBehavior.Init();
        }

        private void OnCoordinatesChanged(Coordinate newCoordinates)
        {
            if (_coordinate.gridPosition != newCoordinates.gridPosition)
            {
                return;
            }
            
            SetCoordinate(newCoordinates);
            ChangeBehaviour(_gridManager.GetBlockBehaviour(newCoordinates.startBlockType));
            SetBlockSprite(_blocksData.GetBlockConfig(newCoordinates.startBlockType).BlockSprite);
        }
        
        private void OnAnimateFill(Coordinate from, Coordinate destination)
        {
            if (_coordinate.gridPosition != destination.gridPosition)
            {
                return;
            }
            
            _blockBehavior.OnAnimateFill(from, destination);
        }
        
        private void  OnCoordinateBlasted(Vector2Int gridPosition, BlockId blastedBlockId, bool isGoal, BlastReason blastReason)
        {
            var self = _coordinate.gridPosition == gridPosition;
            
            if (self && isGoal)
            {
                _uiEntityManager.Show<FloatingBlockPanel>(window
                    => window.Init(new FloatingBlockPanelViewModel(_blocksData.GetBlockConfig(blastedBlockId).BlockSprite,
                        BlockRectPosition, _gameManager.GetGoalPosition(blastedBlockId), BlockSize, scale: Vector3.one)));
            }
            
            _blockBehavior.OnCoordinateBlasted(gridPosition, self, blastedBlockId, isGoal, blastReason);
        }
        
        private void OnAnimateFall(Coordinate destination)
        {
            if (_coordinate.gridPosition != destination.gridPosition)
            {
                return;
            } 
            
            _blockBehavior.OnAnimateFall(destination);
        }
        
        private void SetCoordinate(Coordinate coordinate) => _coordinate = coordinate;

        private void SetBlockSprite(Sprite sprite) => _blockSprite.Value = sprite;
    }
}
