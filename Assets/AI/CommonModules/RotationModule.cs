using UnityEngine;
using System.Collections;

public class RotationModule : MonoBehaviour {

	public float rotateDuration = 0.2f;
	public float rotateLerp = 0.5f;
	public float maxDegrees = 1f;
	public bool zeroY = true;

	public void RotateTowards(Vector3 dir)
	{
		if (zeroY) {
			dir.y = 0;
			dir.Normalize();
		}

		var targetRot = Quaternion.LookRotation(dir);

		// calc. degrees between the two rotations
		var ang = Quaternion.Angle(transform.rotation, targetRot);
		// we want max. X out of ang, so we lerp by x/ang => if x/ang > 1, we rotate full amount.
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, maxDegrees / ang);

		//StopCoroutine("RotateTowardsCoroutine");
		//StartCoroutine("RotateTowardsCoroutine", dir);
	}

	private IEnumerator RotateTowardsCoroutine(Vector3 dir)
	{
		var initRot = transform.rotation;
		var t = 0f;
		while (t < rotateDuration) {
			t += Time.deltaTime;
			var easeT = t / rotateDuration;
			transform.rotation = Quaternion.Lerp(initRot, Quaternion.LookRotation(dir), easeT);
			yield return 0;
		}
	}


}
