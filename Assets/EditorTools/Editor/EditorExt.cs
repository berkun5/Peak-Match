using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace EditorTools.Editor
{
	public static class EditorExt
	{
		private const int IndentationWidth = 16;
		private static readonly List<int> Indentations = new();

		private static void BeginBoxGroup() => BeginBoxGroup(EditorStyles.helpBox);

		private static void BeginBoxGroup(GUIStyle style)
		{
			var indentLevel = EditorGUI.indentLevel;

			GUILayout.BeginHorizontal();
			GUILayout.Space(indentLevel * IndentationWidth);
			GUILayout.BeginVertical(style);

			Indentations.Add(indentLevel);
			EditorGUI.indentLevel = 0;
		}

		private static void EndBoxGroup()
		{
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			var lastIndex = Indentations.Count - 1;
			EditorGUI.indentLevel = Indentations[lastIndex];
			Indentations.RemoveAt(lastIndex);
		}

		private static AnimBool FoldoutHeader(string label, AnimBool foldout)
		{
			if (foldout.target)
			{
				foldout.target = GUILayout.Toggle(foldout.target, label, EditorStyles.foldoutHeader);
			}
			else
			{
				EditorGUI.BeginDisabledGroup(false); 
				foldout.target = GUILayout.Toggle(foldout.target, label, EditorStyles.foldoutHeader);
				EditorGUI.EndDisabledGroup();
			}

			return foldout;
		}

		public static T FoldoutObject<T>(string label, ref AnimBool foldout, Object obj, UnityEditor.Editor objEditor, int indentAmount = 1) where T : Object
		{
			EditorGUILayout.BeginHorizontal(); 
			GUILayout.Space(EditorGUI.indentLevel * IndentationWidth - IndentationWidth); 
			foldout = FoldoutHeader(label, foldout); 
			//can't update object value once its changed properly, disabling for now.. (gonna expose object field on target instead)
			//obj = (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects: false);
			EditorGUILayout.EndHorizontal();

			if (EditorGUILayout.BeginFadeGroup(foldout.faded))
			{
				DrawFoldoutObjectContents(obj, objEditor, indentAmount);
			}
			
			EditorGUILayout.EndFadeGroup();
			return (T)obj;
		}

		private static void DrawFoldoutObjectContents(Object obj, UnityEditor.Editor objEditor, int indentAmount)
		{
			if (obj == null)
			{
				EditorGUILayout.HelpBox("Null Object.", MessageType.Warning);
			}
			else
			{
				if (objEditor == null)
				{
					EditorGUILayout.HelpBox("Editor Not Initialized!", MessageType.Error);
				}
				else
				{
					BeginBoxGroup();
					EditorGUI.indentLevel += indentAmount;
					objEditor.OnInspectorGUI();
					EditorGUI.indentLevel -= indentAmount;
					EndBoxGroup();
				}
			}
		}
	}
}