using System.Collections.Generic;
using Logger;
using UnityEngine;

namespace DataManagement
{
    //root for all local data.
    [CreateAssetMenu(fileName = "LocalDataCollection", menuName = "ScriptableObjects/LocalDataCollection")]
    public class LocalDataCollection : ScriptableObject
    {
        [SerializeField] private List<LocalData> allLocalData;
        
        public T GetData<T>() where T : LocalData
        {
            foreach (LocalData data in allLocalData)
            {
                if (data is T)
                {
                    return (T)data;
                }
            }
            DevLog.LogError($"Data type of {typeof(T)} does not exist in the local data collection.");
            return null;
        }
    }
}
