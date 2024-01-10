using Blocks.Enum;
using Config;
using GameConfig.Enum;
using GameServices;
using GameServices.ServiceLocator;
using Managers.Base;
using Managers.UI;
using UI.Grid;
using UI.TopWindow;
using UnityEngine;

namespace Managers
{
    public class GameManager : ManagerBase
    {
        private UIEntityManager _uiEntityManager;
        private SettingsManager _settingsManager;
        private GridManager _gridManager;
        private GameData _gameData;
        private TopWindow _topWindow;
        
        public override void Init()
        {

        }

        public override void LateStart()
        {
            var uiService = GameServiceLocator.GetService<UIServiceProvider>();
            
            _uiEntityManager = uiService.GetManager<UIEntityManager>();
            _gridManager = uiService.GetManager<GridManager>();
            
            _settingsManager = GameServiceLocator.GetService<PersistentServiceProvider>()
                .GetManager<SettingsManager>();
            
            _gameData = _settingsManager.LocalData.GetData<GameData>();
            
            StartGame();
        }
        
        private void StartGame()
        {
            
            switch (_gameData.GetGameState())
            {
                default:
                case GameState.LevelSelection:
                    break;
                case GameState.MatchGameplay:
                    StartNewLevel();
                    break;
            }
        }

        private void StartNewLevel()
        {
            var activeLevelConfig = _settingsManager.LocalData.GetData<LevelsData>().GetLevelConfig(_gameData.GetActiveLevel());
            var levelCoordinates = _gridManager.GenerateLevelGrid(activeLevelConfig, _uiEntityManager.MainCanvasRect);
            
            _uiEntityManager.Show<GridWindow>(window
                => window.Init(new GridWindowViewModel(levelCoordinates, _gridManager)));
            
            _topWindow = _uiEntityManager.Show<TopWindow>(window => 
                window.Init(new TopWindowViewModel(_settingsManager, _gridManager)));
        }

        public Vector3 GetGoalPosition(BlockId blockId)
        {
            return _topWindow.GetGoalPosition(blockId);
        }
    }
}

