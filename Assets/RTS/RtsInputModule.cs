using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class RtsInputModule : MonoBehaviour {

	// mouse positions: left and right button.
	private Vector3[] startPos = new Vector3[2], endPos = new Vector3[2];

	/// <summary>
	/// for mouse move event.
	/// </summary>
	private Vector3 oldMousePos;

	public class InputData
	{
		public Vector3 startPos;
		public Vector3 endPos;
		public InputData(Vector3 startPos, Vector3 endPos)
		{
			this.startPos = startPos;
			this.endPos = endPos;
		}

		public InputData ToWorld()
		{
			return new RtsInputModule.InputData(GetPosOnGround(startPos), GetPosOnGround(endPos));
		}
		
		Vector3 GetPosOnGround(Vector3 mousePos)
		{
			return Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePos))
				.Aggregate((r1, r2) => (r1.point - Camera.main.transform.position).sqrMagnitude > (r2.point - Camera.main.transform.position).sqrMagnitude ? r2 : r1).point;
		}
	}

	public delegate void InputHandler(InputData data);
	public static event InputHandler LeftMouseDown, RightMouseDown, LeftMouseUp, RightMouseUp, LeftMouseMove, RightMouseMove;

	void Update()
	{
		for (int mouseButton = 0; mouseButton < 2; mouseButton++) {
			if (Input.GetMouseButtonDown(mouseButton)) {
				startInput(mouseButton);
				
			} else if (Input.GetMouseButtonUp(mouseButton)) {
				endInput(mouseButton);

			} else if (Input.GetMouseButton(mouseButton)) {
				if (Input.mousePosition != oldMousePos) {
					moveInput(mouseButton);
				}
			}
		}
		oldMousePos = Input.mousePosition;
	}

	private void startInput(int button)
	{
		startPos[button] = GetInputPoint();
		var data = new InputData(startPos[button], startPos[button]);
		if (button == 0) {
			if (LeftMouseDown != null) {
				LeftMouseDown(data);
			}
		} else if (button == 1) {
			if (RightMouseDown != null) {
				RightMouseDown(data);
			}
		}
	}

	private void endInput(int button)
	{
		endPos[button] = GetInputPoint();
		var data = new InputData(startPos[button], endPos[button]);
		if (button == 0) {
			if (LeftMouseUp != null) {
				LeftMouseUp(data);
			}
		} else if (button == 1) {
			if (RightMouseUp != null) {
				RightMouseUp(data);
			}
		}
	}

	private void moveInput(int button)
	{
		endPos[button] = GetInputPoint();
		var data = new InputData(startPos[button], endPos[button]);
		if (button == 0) {
			if (LeftMouseMove != null) {
				LeftMouseMove(data);
			}
		} else if (button == 1) {
			if (RightMouseMove != null) {
				RightMouseMove(data);
			}
		}
	}

	public Vector3 GetInputPoint()
	{
		return Input.mousePosition;
	}

}
