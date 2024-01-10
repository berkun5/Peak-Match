using System.Collections;
using DataManagement;
using Managers.Base;
using UnityEngine;

namespace Managers
{
    public class SettingsManager : ManagerBase
    {
        public LocalDataCollection LocalData => localDataCollection;

        [SerializeField] private LocalDataCollection localDataCollection;

        public override void Init()
        {
            
        }

        public override void LateStart()
        {
            
        }
    }
}
