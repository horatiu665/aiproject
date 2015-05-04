using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class UnitSelectionModule : MonoBehaviour
{

	public Transform selectionHandle;
	public List<Transform> selection = new List<Transform>();
	
	void Start()
	{
		RtsInputModule.LeftMouseMove += Selection;
	}

	void Update()
	{

	}

	void Selection(RtsInputModule.InputData data)
	{
		// select all objects which contain AIController and are within the cuboid [startPos, endPos]

		// remove selection handle
		foreach (var s in selection) {
			RemoveSelection(s);
		}

		selection = GameObject.FindObjectsOfType<PlantmanAI4.ControllerState>().Where(ai =>
			IsInsideSelection(ai.transform.position, data.startPos, data.endPos)).Select(ai => ai.transform).ToList();

		// add selection handle
		foreach (var s in selection) {
			AddSelection(s);
		}
	}

	public void AddSelection(Transform t)
	{
		var sh = Instantiate(selectionHandle, t.position, t.rotation) as Transform;
		sh.name = "SelectionHandle";
		sh.parent = t;
	}

	public void RemoveSelection(Transform t)
	{
		Destroy(t.FindChild("SelectionHandle").gameObject);
	}

	private bool IsInsideSelection(Vector3 pos, Vector3 start, Vector3 end)
	{
		// true when rays surround targeted object
		var posOnScreen = Camera.main.WorldToScreenPoint(pos);

		var minX = Mathf.Min(start.x, end.x);
		var maxX = Mathf.Max(start.x, end.x);
		var minY = Mathf.Min(start.y, end.y);
		var maxY = Mathf.Max(start.y, end.y);

		return (posOnScreen.x > minX && posOnScreen.x < maxX && posOnScreen.y > minY && posOnScreen.y < maxY);

	}
}
