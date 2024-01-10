using System.Collections.Generic;
using UnityEngine;

namespace Logger
{
    public static class DevLog
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        //history can be used for debug list view
        private static readonly List<LogEntity> History = new();
#endif
        
        public static void Log(object text, bool isImportant = false)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Log($"DevLog :: {text}");
            AddLogToHistory(text, "Log", Color.white);
#else
            if (isImportant)
            {
                // @server api
            }
#endif
        }
        
        public static void LogWarning(object text, bool isImportant = false)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarning($"DevLog :: {text}");
            AddLogToHistory(text, "Warning", Color.yellow);
#else
            if (isImportant)
            {
                // @server api
            }
#endif
        }
        
        public static void LogError(object text, bool isImportant = false)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogError($"DevLog :: {text}");
            AddLogToHistory(text, "Error", Color.red);
#else
            if (isImportant)
            {
                // @server api
            }
#endif
        }
        
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private static void AddLogToHistory(object text, string tag, Color color)
        {
            History.Add(new LogEntity($"[{History.Count}] :: [{tag}] :: [{Time.unscaledTime}] :: {text}", color));
        }
#endif
    }
}
