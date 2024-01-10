using System.Collections.Generic;
using DataManagement;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "NewLevelsData", menuName = "ScriptableObjects/Lists/LevelsData")]
    public class LevelsData : LocalData
    {
        [SerializeField] private List<LevelConfig> allLevels = new();
        
        public LevelConfig GetLevelConfig(int levelIndex)
        {
            return allLevels.Find(level => level.LevelIndex == levelIndex);
        }
    }
}
