using System;
using System.Collections.Generic;
using GameServices.ServiceLocator;
using Logger;
using Managers.Base;
using Managers.Interface;
using UnityEngine;

namespace GameServices.Base
{
    public class GameServiceBase : MonoBehaviour
    {
        [SerializeField] private List<ManagerBase> managers = new();
        private readonly List<IManagerUpdate> _updateListeners = new();
        private Dictionary<string, ManagerBase> _initializedManagers = new();
        private bool _lateStartedManagers;
        
        private void Awake() => Init();
        
        private void OnDestroy() => Dispose();

        private void Start() => GameServiceLocator.LateStartAllServices();

        private void Update()
        {
            foreach (var updateListener in _updateListeners)
            { 
                updateListener.UpdateManager();
            }
        }

        public void LateStartServiceProvider()
        {
            if (_lateStartedManagers)
            {
                return;
            }
            
            _lateStartedManagers = true;
            foreach (var manager in managers)
            {
                manager.LateStart();
                if (manager is IManagerUpdate updateListener)
                {
                    _updateListeners.Add(updateListener);
                }
            }
        }
        
        private void Init()
        {
            GameServiceLocator.Register(this);
            
            _initializedManagers = new Dictionary<string, ManagerBase>();
            foreach (var manager in managers)
            {
                manager.Init();
                var key = manager.GetType().Name;
                _initializedManagers.Add(key, manager);
            }
        }

        private void Dispose() => GameServiceLocator.Unregister(this);
        
        public T GetManager<T>() where T : ManagerBase
        {
            var key = typeof(T).Name;
            var typeName = GetType().Name;
            
            if (_initializedManagers == null)
            {
                DevLog.LogError($"Managers are not initialized correctly in {typeName}");
            }
            else
            {
                if (_initializedManagers.TryGetValue(key, out var manager))
                {
                    return (T) manager;
                }
            }
            
            throw new Exception($"Invalid key: {key} for {typeName}. Make sure you are searching the Manager in the correct ServiceProvider.");
        }
    }
}
