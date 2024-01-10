using System;
using System.Collections.Generic;
using System.Linq;
using Logger;
using Managers.Base;
using UI;
using UI.ExtensionsAndHelpers;
using UnityEngine;

namespace Managers.UI
{
    public class UIEntityManager : ManagerBase
    {
        public RectTransform MainCanvasRect => mainCanvasRect;
        public Camera MainCamera => mainCamera;
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private RectTransform mainCanvasRect;
        [SerializeField] private UIEntityPool pool;
        
        private readonly Dictionary<Type, List<UIEntity>> _activeEntities = new();
        private readonly Dictionary<Type, List<UIEntity>> _inactiveEntities = new();
        private readonly Dictionary<Type, GameObject> _sceneUIByType = new();
        
        public override void Init()
        {
            PrepareUIEntities();
        }

        public override void LateStart()
        {
            
        }
        
        public T Show<T>(Action<T> onCreated = null) where T : UIEntity
        {
            var uiEntityToShow = GetEntity<T>();

            if (uiEntityToShow == null)
            {
                DevLog.LogWarning($"Failed to show UIEntity {typeof(T)} because pool returned null.");
                return null;
            }

            var uiEntityTransform = uiEntityToShow.transform;
            
            uiEntityTransform.SetParent(MainCanvasRect);
            uiEntityTransform.SetSiblingIndex(transform.childCount - 1);
            uiEntityTransform.transform.localScale = Vector3.one;
            uiEntityTransform.localPosition = Vector3.zero;
            uiEntityToShow.gameObject.SetActive(true);
           
            var uiEntityRect = uiEntityTransform as RectTransform;
            if (uiEntityRect != null)
            {
                RectTransformExtensions.CenterAndExpandRect(uiEntityRect);
            }

            onCreated?.Invoke(uiEntityToShow);
            return uiEntityToShow;
        }

        public void Hide<T>() where T : UIEntity
        {
            var entityType = typeof(T);
            if (_activeEntities.TryGetValue(entityType, out var entities))
            {
                    // Find the first entity of the specified type
                    var uiEntityToHide = entities.FirstOrDefault(e => e.GetType() == entityType);
                    
                    if (uiEntityToHide == null)
                    {
                        DevLog.LogWarning($"Failed to hide UIEntity, because it's not being shown.");
                        return;
                    }

                    var activeEntityTransform = uiEntityToHide.transform;
                    activeEntityTransform.SetParent(transform);
                    activeEntityTransform.localPosition = Vector3.zero;
                    uiEntityToHide.gameObject.SetActive(false);
                    _activeEntities[entityType].Remove(uiEntityToHide);

                    if (_activeEntities[entityType].Count == 0)
                    {
                        _activeEntities.Remove(entityType);
                    }

                    if (!_inactiveEntities.ContainsKey(entityType))
                    {
                        _inactiveEntities.Add(entityType, new List<UIEntity>());
                    }

                    _inactiveEntities[entityType].Add(uiEntityToHide); 
            }
            else 
            { 
                DevLog.LogWarning($"Can not find active UI to hide."); 
            } 
        }

        private void PrepareUIEntities()
        {
            foreach (var uiPrefab in pool.SceneUIPool)
            {
                var type = uiPrefab.GetComponent<UIEntity>().GetType();
                
                if (!_sceneUIByType.ContainsKey(type))
                {
                    _sceneUIByType.Add(type, uiPrefab.gameObject);

                    if (!uiPrefab.PrepareOnInit)
                    {
                        continue;
                    }
                    
                    var prewarmedEntity = InstantiateUIEntity(uiPrefab.gameObject);
                    var prewarmedEntityTransform = prewarmedEntity.transform;
                        
                    prewarmedEntityTransform.SetParent(transform);
                    prewarmedEntityTransform.localPosition = Vector3.zero;
                    prewarmedEntityTransform.gameObject.SetActive(false);
                        
                    var entityType = prewarmedEntity.GetType();
                    if (!_inactiveEntities.ContainsKey(entityType))
                    {
                        _inactiveEntities.Add(entityType, new List<UIEntity>());
                    }

                    _inactiveEntities[entityType].Add(prewarmedEntity);
                }
                else
                {
                    Debug.LogWarning($"Duplicated UIEntity in sceneUIPool of type {type}.");
                }
            }
        }
        
        private T GetEntity<T>() where T : UIEntity
        {
            var entityType = typeof(T);
            UIEntity inactiveEntity;

            if (_inactiveEntities.ContainsKey(entityType) && _inactiveEntities[entityType].Count > 0)
            {
                inactiveEntity = _inactiveEntities[entityType][0];
                _inactiveEntities[entityType].RemoveAt(0);
            }
            else
            {
                inactiveEntity = InstantiateUIEntity(_sceneUIByType[entityType]);
            }

            if (!_activeEntities.ContainsKey(entityType))
            {
                _activeEntities.Add(entityType, new List<UIEntity>());
            }

            _activeEntities[entityType].Add(inactiveEntity);
            inactiveEntity.gameObject.SetActive(false);

            return (inactiveEntity) as T;
        }
        
        private UIEntity InstantiateUIEntity(GameObject ui)
        {
            var entity = Instantiate(ui);
            return entity.GetComponent<UIEntity>();
        }
    }
}
