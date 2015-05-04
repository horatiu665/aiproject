using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class Jump : MonoBehaviour
{
	public List<ContactPoint> currentContacts = new List<ContactPoint>();

	public bool control = false;

	public bool IsGrounded()
	{
		return Physics.SphereCastAll(transform.position, 0.2f, Vector3.down, 1.01f).Any(rh => rh.transform != transform);
		//return (currentContacts.Count > 0) && currentContacts.Any(cc => cc.point.y < transform.position.y - 0.75f);
	}

	private Rigidbody rigidbody;
	public float height;

	public float acceleration, maxSpeed;

	private Transform forward;

	// Use this for initialization
	void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		forward = transform.GetComponentInChildren<Camera>().transform;

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (control) {
			var fw = forward.forward;
			fw.y = 0;
			fw.Normalize();
			var fr = forward.right;
			fr.y = 0;
			fr.Normalize();
			var mot = fr * Input.GetAxis("Horizontal") * acceleration + acceleration * Input.GetAxis("Vertical") * fw;

			Debug.DrawRay(transform.position, rigidbody.velocity, Color.blue, 0.5f);
			// if speed in direction of input is smaller than limit, add force to match it.
			if ((rigidbody.velocity).magnitude < maxSpeed) {
				rigidbody.AddForce(mot);
			}

			if (Input.GetButtonDown("Jump") && IsGrounded()) {
				rigidbody.AddForce(Vector3.up * height, ForceMode.VelocityChange);

			}
		}
	}

	void OnCollisionEnter(Collision c)
	{
		foreach (var con in c.contacts) {
			currentContacts.Add(con);
		}
	}

	void OnCollisionExit(Collision c)
	{
		currentContacts.RemoveAll(cc => cc.otherCollider == c.collider);
	}

	void OnGUI()
	{
		GUI.Button(new Rect(0, 0, 50, 30), currentContacts.Count.ToString());

	}
}
