using UnityEngine;
using System.Collections;
using System.Linq;

public class PositionOnObject : MonoBehaviour
{

	public Transform target;
	[Tooltip("Leave at 0,0,0 to position towards target position. set to global position to position towards direction")]
	public Vector3 direction = new Vector3(0, 0, 0);
	public Transform parentTo;
	public float height = 0;
	public float maxDistance = 500;

	// Use this for initialization
	void Start()
	{
		PositionOn(transform, target, direction, maxDistance, height, parentTo);

	}

	public static bool PositionOn(Transform transform, Transform target, Vector3 direction, float maxDistance, float height = 0, Transform parentTo = null)
	{
		Vector3 raycastDir;
		if (direction != Vector3.zero) {
			raycastDir = direction;
		} else {
			raycastDir = target.position - transform.position;
		}

		if (Physics.Raycast(transform.position, raycastDir, maxDistance)) {
			// instead of this, find better way to reposition stuff.
			RaycastHit[] rg = Physics.RaycastAll(transform.position, raycastDir, 500);
			if (rg.Any(r => r.transform == target)) {

				// rotate so it is looking towards normal of intersection with target
				transform.rotation = LookAtWithY(rg.First(r => r.transform == target).normal);// *Quaternion.Euler(0, 90, 0);

				if (height != 0) {
					// move over to target
					EaseTo(transform, rg.First(r => r.transform == target).point + (transform.position - target.position).normalized * height);
				} else {
					EaseTo(transform, rg.First(r => r.transform == target).point);
				}
				//transform.position = rg.First(r => r.transform.tag == "Terrain").point;
			} else {
				return false;
			}
		} else {
			return false;
		}

		if (parentTo != null) {
			transform.parent = parentTo;
		}

		return true;
	}

	public static Vector3 GetPositionOnObject(Vector3 origin, Vector3 direction, Transform target = null)
	{
		RaycastHit[] rg = Physics.RaycastAll(origin, direction, 5000);
		if ((target == null && rg.Any())) {
			return rg.First().point;
		} else if (target != null && rg.Any(r => r.transform == target)) {
			return rg.First(r => r.transform == target).point;
		} else {
			return origin;
		}
	}

	// returns rotation which has the -Y axis pointing towards target, like transform.LookAt has the Z axis.
	static Quaternion LookAtWithY(Vector3 up)
	{
		// fwd does not matter as long as we have up
		// we must find fwd perpendicular to up, to use Quaternion.lookrotation
		Vector3 fwd = Quaternion.Euler(90, 0, 0) * up;
		return Quaternion.LookRotation(fwd, up);

	}

	static void EaseTo(Transform transform, Vector3 finalPos)
	{
		//Vector3 initPos = transform.position;

		transform.position = finalPos;

	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.position - transform.up);
		Gizmos.DrawSphere(transform.position, 0.05f);
	}

}
