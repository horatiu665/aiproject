using UnityEngine;
using System.Collections;

public class AddBobOnLand : MonoBehaviour
{

	public Rigidbody bobbingRigidbody;
	public Vector3 jumpForce;
	public Vector3 landForce;

	void OnJump()
	{
		if (Input.GetButtonDown("Jump")) {
			bobbingRigidbody.AddForce(jumpForce, ForceMode.Impulse);
		}
	}

	void OnLand()
	{
		// add bob on rigidbody of camera
		bobbingRigidbody.AddForce(landForce, ForceMode.Impulse);
	}
}
