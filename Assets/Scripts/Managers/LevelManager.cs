using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameConfig.Enum;
using GameServices;
using GameServices.ServiceLocator;
using Logger;
using Managers.Base;

namespace Managers
{
    public class LevelManager : ManagerBase
    {
        private Dictionary<GameState, GameScene> _sceneStatePair = new();
        private GameData _gameData;
        public override void Init()
        {
            _sceneStatePair = new Dictionary<GameState, GameScene>
            {
                { GameState.LevelSelection, GameScene.LevelSelectionScene },
                { GameState.MatchGameplay, GameScene.MatchScene },
            };
        }

        public override void LateStart()
        {
            _gameData = GameServiceLocator.GetService<PersistentServiceProvider>()
                .GetManager<SettingsManager>().LocalData.GetData<GameData>();
            LoadScene(_gameData.GetGameState());
        }

        public void LoadScene(GameState gameState)
        {
            if (_sceneStatePair.TryGetValue(gameState, out var gameScene))
            {
                StartCoroutine(LoadGameSceneAsync(gameScene));
                _gameData.SetGameState(gameState);
                return;
            }
            
            DevLog.LogWarning($"Could not load the Scene for the {gameState} state.");
        }
        
        private IEnumerator LoadGameSceneAsync(GameScene loadSceneCandidate)
        {
            if (loadSceneCandidate == GameScene.PersistentScene)
            {
                DevLog.LogWarning($"Cannot load persistentScene.");
                yield break;
            }
            
            var gameScenes = System.Enum.GetValues(typeof(GameScene));
            foreach (GameScene scene in gameScenes)
            {
                var sceneName = scene.ToString();
                var sceneLoaded = SceneManager.GetSceneByName(sceneName).isLoaded;
                
                if (scene == GameScene.PersistentScene || !sceneLoaded)
                {
                    continue;
                }
                
                var unloadOperation = UnloadSceneAsyncOperation(sceneName);
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }

            var loadOperation = LoadSceneAsyncOperation(loadSceneCandidate.ToString());
            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }

        private AsyncOperation LoadSceneAsyncOperation(string loadScene)
        {
            return SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        }

        private AsyncOperation UnloadSceneAsyncOperation(string unloadScene)
        {
            return SceneManager.UnloadSceneAsync(unloadScene);
        }
        
    }
}
