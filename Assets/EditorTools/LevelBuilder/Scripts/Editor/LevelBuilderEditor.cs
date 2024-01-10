#if UNITY_EDITOR

using Config;
using EditorTools.Editor;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace EditorTools.LevelBuilder.Scripts.Editor
{
    [CustomEditor(typeof(LevelBuilder))]
    public sealed class LevelBuilderEditor : UnityEditor.Editor
    {
        [System.NonSerialized] private AnimBool _skinEditorFoldoutLevelConfig = new AnimBool
        {
            value = true,
            target = true,
            speed = 10f,
        };
        [System.NonSerialized] private AnimBool _skinEditorFoldoutGameData = new AnimBool
        {
            value = true,
            target = true,
            speed = 10f,
        };
        
        private Scripts.LevelBuilder _levelBuilder;

        private SerializedProperty _gameData;
        private SerializedProperty _activeLevelConfig;
        private SerializedProperty _blockPrefab;
        private SerializedProperty _canvasRect;
        private SerializedProperty _randomizationBlockIdPool;
        private UnityEditor.Editor _levelConfigSkinEditor;
        private UnityEditor.Editor _gameConfigSkinEditor;
        
        private void OnEnable()
        {
            if (target != null)
            {
                _levelBuilder = target as Scripts.LevelBuilder;
            }

            _gameData = serializedObject.FindProperty("gameData");
            _activeLevelConfig = serializedObject.FindProperty("activeLevelConfig");
            _blockPrefab = serializedObject.FindProperty("blockPrefab");
            _canvasRect = serializedObject.FindProperty("canvasRect");
            _randomizationBlockIdPool = serializedObject.FindProperty("randomizationBlockIdPool");
            
            if (_levelBuilder != null)
            {
                RefreshSkinEditor(_levelBuilder.activeLevelConfig, _levelBuilder.gameData);
                _levelBuilder.Init();
            }
        }
        
        public override void OnInspectorGUI()
        {
            CheckAndApplyLevelConfigChanges();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_gameData, true);
            EditorGUILayout.PropertyField(_blockPrefab, true);
            EditorGUILayout.PropertyField(_canvasRect, true);
            EditorGUILayout.PropertyField(_randomizationBlockIdPool, true);
            serializedObject.ApplyModifiedProperties();
            
            if (!_levelBuilder.blockPrefab)
            {
                EditorGUILayout.HelpBox("Block Prefab to instantiate is missing. Please insert one.", MessageType.Error);
                return;
            }
            
            if (!_levelBuilder.activeLevelConfig)
            {
                EditorGUILayout.HelpBox("LevelConfig is missing. Please insert one.", MessageType.Error);
                return;
            }
            
            if (!_levelBuilder.gameData)
            {
                EditorGUILayout.HelpBox("GameData Config is missing. Please insert one.", MessageType.Error);
                return;
            }
            
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Reset Grid",GUILayout.Height(40)))
            {
                _levelBuilder.ClearGrid(false);
            }
            
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Randomize Blocks",GUILayout.Height(40)))
            {
                _levelBuilder.UpdateGrid(false, true);
            }
            
            DrawConfigs();
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            { 
                _levelBuilder.UpdateGrid();
            }
        }

        private void CheckAndApplyLevelConfigChanges()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_activeLevelConfig, new GUIContent("Level Config"), true, GUILayout.Height(40));
            serializedObject.ApplyModifiedProperties();
            
            if (!EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }
            
            if (_levelBuilder.activeLevelConfig == null)
            {
                //clear children
                _levelBuilder.ClearGrid();
                serializedObject.ApplyModifiedProperties();
                return;
            }
            
            if (_levelBuilder.gameData == null)
            {
                return;
            }
            
            _levelBuilder.UpdateGrid(true);
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawConfigs()
        {
            var gameData = EditorExt.FoldoutObject<GameData>("GameDataConfig", ref _skinEditorFoldoutGameData, _levelBuilder.gameData, _gameConfigSkinEditor);
            EditorGUILayout.Space(10);
            var activeLevelConfig = EditorExt.FoldoutObject<LevelConfig>("LevelDataConfig", ref _skinEditorFoldoutLevelConfig, _levelBuilder.activeLevelConfig, _levelConfigSkinEditor);
            
            if (activeLevelConfig)
            {
                EditorUtility.SetDirty(activeLevelConfig);
            }
            
            if (gameData)
            {
                EditorUtility.SetDirty(gameData);
            }

            serializedObject.ApplyModifiedProperties();
            RefreshSkinEditor(activeLevelConfig, gameData);
        }
        
        private void RefreshSkinEditor(LevelConfig levelConfig, GameData gameData)
        {
            _levelConfigSkinEditor = levelConfig ? CreateEditor(levelConfig) : null;
            _gameConfigSkinEditor = gameData ? CreateEditor(gameData) : null;;
        }
    }
}
#endif