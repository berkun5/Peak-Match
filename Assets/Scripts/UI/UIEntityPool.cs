using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "NewUIEntityPool", menuName = "ScriptableObjects/Lists/UIEntityPool", order = 99)]
    public class UIEntityPool : ScriptableObject
    {
        public List<UIEntity> SceneUIPool => sceneUIPool;
        [SerializeField] private List<UIEntity> sceneUIPool = new();
    }
}
