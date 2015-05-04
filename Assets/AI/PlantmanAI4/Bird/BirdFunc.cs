using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// contains functionality for bird. Also contains tweakables
/// </summary>
public class BirdFunc : MonoBehaviour
{

	public float distanceToGround = 5f;
	public AnimationCurve descentSpeed;

	public void FlyDown(float deltaTime)
	{

		// fly downward until raycast hits ground X units below
		var ra = Physics.RaycastAll(transform.position, Vector3.down, 100);
		if (ra.Any()) {
			var below = ra.Aggregate((h1, h2) => h1.point.y > h2.point.y ? h1 : h2).point;
			// distance to point below
			var dist = (transform.position - below).magnitude - distanceToGround;
			transform.position += deltaTime * Vector3.down * descentSpeed.Evaluate(dist);
		}

	}

}
