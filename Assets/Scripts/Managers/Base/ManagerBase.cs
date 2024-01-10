using UnityEngine;

namespace Managers.Base
{
    public abstract class ManagerBase : MonoBehaviour
    {
        public abstract void Init();
        public abstract void LateStart();
    }
}
