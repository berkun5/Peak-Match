using System;
using System.Collections.Generic;
using GameServices.Base;
using Logger;

namespace GameServices.ServiceLocator
{
    public static class GameServiceLocator
    {
        private static Dictionary<string, GameServiceBase> _services = new();
        private static bool _initialized;
        
        private static void Init()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            _services = new Dictionary<string, GameServiceBase>();
            _services.Clear();
        }
        
        public static void LateStartAllServices()
        {
            foreach (var serviceProvider in _services)
            {
                serviceProvider.Value.LateStartServiceProvider();
            }
        }
        
        public static T GetService<T>() where T : GameServiceBase
        {
            if (!_initialized)
            {
                throw new Exception("gameServiceLocator has not been initialized");
            }
            
            var key = typeof(T).Name;
            if (_services.TryGetValue(key, out var service))
            {
                return (T)service;
            }
            
            throw  new Exception($"getting service failed. {key} is not registered");
        }
        
        public static void Register<T>(T service) where T : GameServiceBase
        {
            if (!_initialized)
            {
                Init();
            }

            var key = service.GetType().Name;
            DevLog.Log($"registering: {key}.");
            
            if (_services.TryAdd(key, service))
            {
                return;
            }
            
            DevLog.LogWarning($"{key} has been already registered");
        }
        
        public static void Unregister<T>(T service) where T : GameServiceBase
        {
            if (!_initialized)
            {
                Init();
            }

            var key = service.GetType().Name;
            if (_services.ContainsKey(key))
            {
                _services.Remove(key);
                return;
            }

            DevLog.LogError($"unregister failed. {key} is not registered");
        }
    } 
}
