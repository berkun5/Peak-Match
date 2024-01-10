using System.Collections.Generic;
using Blocks.Enum;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "NewLevelConfig", menuName = "ScriptableObjects/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int LevelIndex => levelIndex;
        public int MovesCount => movesCount;
        public int RowCount => rowCount;
        public int ColumnCount => columnCount;

        [SerializeField] private int levelIndex;
        [SerializeField] private int movesCount;
        [SerializeField, Range(2, 50)] private int rowCount = 2;
        [SerializeField, Range(2, 50)] private int columnCount = 2;
        
        [Space(10)]
        //TODO: cast private get after set dirty implementation for builder editor script
        public List<BlockId> blockPool  = new();
        public List<GoalValue> goalValues  = new();
        public List<Coordinate> gridCoordinates  = new();
    }
}