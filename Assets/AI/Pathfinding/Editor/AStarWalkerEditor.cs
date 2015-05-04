using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AStarWalker))]
public class AStarWalkerEditor : Editor {

	SerializedObject so;
	SerializedProperty prop;

	AStarWalker script;

	void OnEnable()
	{
		script = ((AStarWalker)target);
		so = new SerializedObject(target);

	}

	public override void OnInspectorGUI()
	{
		prop = so.GetIterator();
		// first element
		prop.Next(true);

		System.Reflection.PropertyInfo pihahaha = typeof(AStarWalker).GetProperty("movementSpeed");

		while (prop.NextVisible(false)) {
			var t = script.GetType();
			var proppp = t.GetProperty(prop.propertyPath);
			if (proppp != null) {
				
			}
			EditorGUILayout.PropertyField(prop, true);
		}

		EditorGUILayout.Space();

		if (GUI.changed) {
			so.ApplyModifiedProperties();
		}
	}
}
