using UnityEngine;
using System.Collections;

public class NodeGizmo : MonoBehaviour {

	public float size = 0.2f;
	public bool showNeighbors = false;
	public Gradient gizmosColor = new Gradient();

	void OnDrawGizmos()
	{
		var cnode = GetComponent<ConnectedNode>();
		if (cnode != null) {
			Gizmos.color = gizmosColor.Evaluate(Mathf.Clamp01(cnode.GetConnectedZone() / (float)cnode.handler.totalConnectedZones));
			Gizmos.DrawSphere(transform.position, size);
			if (showNeighbors) {
				Gizmos.color = Color.green;
				foreach (var n in GetComponent<ConnectedNode>().neighbors) {
					Gizmos.DrawSphere(n.transform.position, size * 1.5f);
				}
			}
		} else {
			Gizmos.color = gizmosColor.Evaluate(0);
			Gizmos.DrawSphere(transform.position, size);
			
		}
	}
}
