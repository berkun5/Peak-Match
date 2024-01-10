using System.Collections.Generic;
using Blocks;
using Blocks.Interface;
using Config;
using GameServices;
using GameServices.ServiceLocator;
using Managers;
using Managers.UI;
using UnityEngine;

namespace UI.Grid
{
    public class GridWindowViewModel : IGridWindowViewModel
    {
        public List<IBlockViewModel> GridBlockViewModels { get; } = new();
        public BlocksData BlocksData { get; }
        
        private readonly GridManager _gridManager;
        private readonly UIEntityManager _uiEntityManager;
        private readonly CommandManager _commandManager;
        private readonly GameManager _gameManager;
        private readonly List<Coordinate> _gridCoordinates;

        public GridWindowViewModel(List<Coordinate> gridCoordinates, GridManager gridManager)
        {
            _gridCoordinates = gridCoordinates;
            _gridManager = gridManager;

            var uiServiceProvider = GameServiceLocator.GetService<UIServiceProvider>();
            var settingsManager = GameServiceLocator.GetService<PersistentServiceProvider>().GetManager<SettingsManager>();
            _uiEntityManager = uiServiceProvider.GetManager<UIEntityManager>();
            _commandManager = GameServiceLocator.GetService<CommandServiceProvider>().GetManager<CommandManager>();
            _gameManager = GameServiceLocator.GetService<GameServiceProvider>().GetManager<GameManager>();
            BlocksData = settingsManager.LocalData.GetData<BlocksData>();
            
            InitializeBlockViewModels();
        }
        
        Vector2 IGridWindowViewModel.GridSize(RectTransform canvasRect)
        {
            var blockSize = GridBlockViewModels[0].BlockSize;
            var row = _gridManager.ActiveLevelConfig.RowCount;
            var col = _gridManager.ActiveLevelConfig.ColumnCount;
            var offset = new Vector2(15,15);
            
            return new Vector2(blockSize.x * col, blockSize.y * row) + offset;
        }
        
        private void InitializeBlockViewModels()
        {
            foreach (var coordinate in _gridCoordinates)
            { 
                GridBlockViewModels.Add(GetBlockViewModel(coordinate, BlocksData));
            }
        }
        
        private IBlockViewModel GetBlockViewModel(Coordinate cord, BlocksData blocksData)
        {
            var blockBehaviour = _gridManager.GetBlockBehaviour(cord.startBlockType);
            return new BlockViewModel<IBlockBehavior>(cord.startBlockType, cord, blocksData,
                blockBehaviour, _gridManager, _commandManager, _uiEntityManager, _gameManager);
        }
        

        
    }
}
