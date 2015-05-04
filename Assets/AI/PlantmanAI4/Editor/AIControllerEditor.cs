using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PlantmanAI4
{
	[CustomEditor(typeof(ControllerState))]
	public class ControllerStateEditor : Editor
	{
		public bool statesExpand = true;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var aiController = (ControllerState)target;

			// print list of states
			if (statesExpand = EditorGUILayout.Foldout(statesExpand, "States")) {
				{
					EditorGUI.indentLevel++;

					if (aiController.states.Count == 0) {
						EditorGUILayout.LabelField("No states");
					} else {
						// list each state along with some controls
						foreach (var s in aiController.states) {
							//EditorGUILayout.BeginHorizontal();
							//EditorGUILayout.LabelField(s.GetName() + "\t" + (s.GetPriority()) + "\t" + (s.GetUninterruptible() ? "uninterruptible" : ""));
							EditorGUILayout.ObjectField(s.GetName(), (Object)s, typeof(IState));
							
							//EditorGUILayout.EndHorizontal();
						}
					}

					EditorGUI.indentLevel--;

				}
			}

			if (GUILayout.Button("Clear states")) {
				aiController.states.Clear();
			}

			EditorGUILayout.LabelField("Current state");
			EditorGUI.indentLevel++;
			if (aiController.currentState == null) {
				EditorGUILayout.LabelField("None");
			} else {
				EditorGUILayout.LabelField(aiController.currentState.GetName() + "\t" + (aiController.currentState.GetPriority()) + "\t" + (aiController.currentState.GetUninterruptible() ? "uninterruptible" : ""));
			}
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
		}
	}
}