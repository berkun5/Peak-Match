using DataManagement;
using GameConfig.Enum;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "ScriptableObjects/GameData")]
    public class GameData : LocalData
    {
        public const int MinimumMatchCount = 2;
        public const float BaseFallAnimationDuration = .35f;
        public const float BaseFillAnimationDuration = .35f;
        public const float FloatingPanelAnimationDuration = .7f;
        
        public float GridWidthOffset => gridWidthOffset;
        public float GridHeightOffset => gridHeightOffset;
        public int RocketMatchCount => rocketMatchCount;
        
        [SerializeField, Range(0, 100)] private float gridWidthOffset;
        [SerializeField, Range(0, 100)] private float gridHeightOffset;

        [SerializeField] private int activeLevel;
        [SerializeField] private GameState currentGameState;
        [SerializeField] private int rocketMatchCount = 5;
        
        
        public void SetActiveLevel(int levelIndex) => activeLevel = levelIndex;
        
        public int GetActiveLevel()
        {
            return activeLevel;
        }
        
        public void SetGameState(GameState newGameState) => currentGameState = newGameState;

        public GameState GetGameState()
        {
            return currentGameState;
        }
    }
}
