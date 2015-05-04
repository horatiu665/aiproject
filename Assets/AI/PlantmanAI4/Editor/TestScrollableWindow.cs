using UnityEngine;
using UnityEditor;
using System.Collections;

public class TestScrollableWindow : EditorWindow
{
	/// <summary>
	/// moves all controls by an amount
	/// </summary>
	static Vector2 viewOffset;

	/// <summary>
	/// whenever anything inside editor changes, bounds are recalculated to surround all elements.
	/// offset cannot exit bounds.
	/// </summary>
	static Rect bounds;


	[MenuItem("Window/TestScrollableWindow")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(TestScrollableWindow));
	}

	

	void OnGUI()
	{

		// ######## UTILITY STUFF STARTS HERE ############

		var mp = Event.current.mousePosition;
		var md = Event.current.delta;
		if (Event.current.type == EventType.mouseDrag) {
			viewOffset += md;
			RecalculateBounds();
			Repaint();
		}
		var origin = new Rect(viewOffset.x, viewOffset.y, 0, 0);



		// ######## GUI STARTS HERE ############

		GUI.Button(origin.Add(new Rect(0, 0, 100, 30)), "yooyoy");

	}

	void RecalculateBounds()
	{
		bounds = new Rect(0, 0, this.position.width - 100, this.position.height - 30);


		viewOffset.x = Mathf.Clamp(viewOffset.x, bounds.x, bounds.x + bounds.width);
		viewOffset.y = Mathf.Clamp(viewOffset.y, bounds.y, bounds.y + bounds.height);

	}

}
