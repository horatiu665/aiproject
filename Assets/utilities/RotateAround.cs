using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

	public Vector3 speed;
	public Transform target;
	private Transform fakeObj;

	// Use this for initialization
	void Start()
	{
		if (target == null || target == transform) {
			fakeObj = transform;
		} else {
			var go = new GameObject();
			go.transform.position = target.position;
			fakeObj = go.transform;
			go.name = transform.name + " Rotator Dummy";
			transform.parent = fakeObj;
		}
	}
	
	// Update is called once per frame
	void Update () {
		fakeObj.transform.Rotate(speed);
	}
}
