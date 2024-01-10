using Blocks.Enum;
using UnityEngine;

namespace Config
{
    [System.Serializable]
    public class GoalValue
    {
        public BlockId GoalId => goalId;
        public int Requirement => requirement;

        [SerializeField] private BlockId goalId;
        [SerializeField] private int requirement;
    }
}
