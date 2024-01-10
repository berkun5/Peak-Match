#if UNITY_EDITOR
using System;
using Blocks;
using Blocks.Enum;
using UnityEditor;
using UnityEngine;

namespace EditorTools.LevelBuilder.Scripts.Editor
{
    [CustomEditor(typeof(LevelBuilderBlock))]
    public class LevelBuilderBlockEditor : UnityEditor.Editor
    {
        private LevelBuilderBlock _builderBlock;
        private void OnEnable()
        {
            if (target != null)
            {
                _builderBlock = target as LevelBuilderBlock;
            }
        }
        
        private void OnSceneGUI()
        {
            Handles.BeginGUI();
            
            const int width = 125;
            const int height = 25;

            Vector3 screenPos = HandleUtility.WorldToGUIPoint(_builderBlock.BlockRect.position);
            var rect = new Rect(screenPos.x - width / 2, screenPos.y - height / 2, width, height);
            GUILayout.BeginArea(rect);

            // Display a button to trigger the dropdown
            if (GUILayout.Button("Select BlockId", GUILayout.Width(rect.width), GUILayout.Height(rect.height)))
            {
                var menu = new GenericMenu();
                // Add menu items for each enum value
                foreach (BlockId blockId in Enum.GetValues(typeof(BlockId)))
                {
                    menu.AddItem(new GUIContent(blockId.ToString()), false, () =>
                    {
                        _builderBlock.ChangeId(blockId);
                        SceneView.RepaintAll();
                    });
                }

                menu.ShowAsContext();
            }

            GUILayout.EndArea();

            Handles.EndGUI();
        }
    }
}
#endif