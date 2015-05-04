using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlantmanAI4;

public class PlantmanAIWindow : EditorWindow
{

	Vector2 scrollPosition;
	// how many buttons?
	static int butCount = 0;
	static Rect rect;
	int maxLevels = 10;

	[MenuItem("Window/PlantmanAI")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(PlantmanAIWindow));
	}

	// called 10 times per second to give the chance for Inspector to update
	void OnInspectorUpdate()
	{
		// get hierarchy of AIStates and such, and save their data for display
		if (!IsSelectionAI()) return;

		Repaint();
	}

	IState GetController()
	{
		return Selection.activeTransform.GetComponent<IState>();
	}

	bool IsSelectionAI()
	{
		var sel = Selection.activeTransform;
		if (sel != null) {
			if (sel.GetComponent<ControllerState>()) 
				return true;
		}
		return false;
	}

	List<StateInfo> RecursiveStates(IState controller, int level, List<StateInfo> list)
	{
		if (level >= maxLevels) return list;

		if (controller is ControllerState) {
			var cs = (ControllerState)controller;

			//foldouts[foldoutCount] = EditorGUILayout.InspectorTitlebar(foldouts[foldoutCount++], cs);
			list.Add(new StateInfo(controller, level));

			foreach (var s in cs.states) {
				list.AddRange(RecursiveStates(s, level + 1, new List<StateInfo>()));
			}

		} else {
			list.Add(new StateInfo(controller, level));

		}
		return list;
	}

	void OnGUI()
	{
		if (!IsSelectionAI()) return;

		float butHeight = 16;
		float w = this.position.width + -15;
		rect = new Rect(0, 0, w, butHeight);
		var nextButton = new Rect(0, butHeight, 0, 0);

		// scroll that shit
		scrollPosition = GUI.BeginScrollView(
			new Rect(0, 0, this.position.width, this.position.height),
			scrollPosition,
			new Rect(0, 0, w, this.position.height)
			, false, true);
		butCount = 0;

		// first get state hierarchy, so we know how many states we have.
		// then we draw the buttons with foldouts and all that shit
		var states = RecursiveStates(GetController(), 0, new List<StateInfo>());

		var initLevel = EditorGUI.indentLevel;
		for (int i = 0; i < states.Count; i++) {
			EditorGUI.indentLevel = states[i].level;
			EditorGUILayout.ObjectField(states[i].state.GetName(), (Object)states[i].state, typeof(IState));

		}
		EditorGUI.indentLevel = initLevel;
		
		GUI.EndScrollView();

	}

	public class StateInfo
	{
		public IState state;
		public int level;

		public StateInfo(IState state, int level)
		{
			this.state = state;
			this.level = level;
		}
	}

}
