using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

[ExecuteInEditMode]
public class NodeMaker : MonoBehaviour
{

	public bool clearNodesButton = false;
	public bool createNodesButton = false;
	public float numNodes = 1;
	public Transform node;

	public enum NodePatterns
	{
		Random,
		DistributedRandom,
		RandomAvoided,
		Grid,

	}

	public NodePatterns nodePattern;

	public float bumpiness;
	public AnimationCurve bumpinessCurve;

	// random pattern
	public float range;
	public AnimationCurve distributedRandomCurve;

	public int gridX, gridY;

	void Update()
	{
		if (clearNodesButton) {
			clearNodesButton = false;
			while (transform.childCount > 0) {
				DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
			}
			SceneView.RepaintAll();
		}
		if (createNodesButton) {
			createNodesButton = false;
			float x, dist;
			Transform n = null;
			switch (nodePattern) {
			case NodePatterns.Random:
				for (int i = 0; i < numNodes; i++) {
					x = Random.Range(0, 2 * Mathf.PI);
					dist = Random.Range(0, 1f);
					n = Instantiate(node, transform.position + new Vector3(Mathf.Sin(x), 0, Mathf.Cos(x)) * dist * range, Quaternion.identity) as Transform;
					n.position += bumpiness * Vector3.up * bumpinessCurve.Evaluate(Random.Range(0, 1f));

					n.parent = transform;
					n.name = "Node" + i;
				}
				break;
			case NodePatterns.DistributedRandom:
				for (int i = 0; i < numNodes; i++) {
					x = Random.Range(0, 2 * Mathf.PI);
					dist = distributedRandomCurve.Evaluate(Random.Range(0, 1f));
					n = Instantiate(node, transform.position + new Vector3(Mathf.Sin(x), 0, Mathf.Cos(x)) * dist * range, Quaternion.identity) as Transform;
					n.position += bumpiness * Vector3.up * Random.Range(0, 1f) * bumpinessCurve.Evaluate(dist);

					n.parent = transform;
					n.name = "Node" + i;
				}
				break;
			case NodePatterns.RandomAvoided:
				for (int i = 0; i < numNodes; i++) {
					// find position at largest distance from any existing points. (montecarlo hahahahaha)
					n = Instantiate(node,
						FindRandomAvoidedPosition(
						GetComponentsInChildren<Transform>()), Quaternion.identity) as Transform;

					n.parent = transform;
					n.name = "Node" + i;
				}
				break;
			case NodePatterns.Grid:
				for (int i = 0; i < gridX; i++) {
					for (int j = 0; j < gridY; j++) {
						n = Instantiate(node, transform.position 
							+ new Vector3(i * range * 2 / (float)Mathf.Clamp(gridX-1,1,float.MaxValue),
								0,
								j * range * 2 / (float)Mathf.Clamp(gridY-1, 1, float.MaxValue)
								) - new Vector3(range, 0, range), Quaternion.identity) as Transform;

						n.parent = transform;
						n.name = "Node" + i;

					}
				}

				break;
			default:
				break;

			}

			SceneView.RepaintAll();
		}
	}

	Vector3 FindRandomAvoidedPosition(Transform[] points)
	{
		// get N random positions
		// return the one with the largest distance to the nearest point (calculate all huhuhhuhuhhuhuuh very inefficient
		Vector3 maxPos = Vector3.zero;
		float maxDist = 0;
		for (int i = 0; i < Mathf.Clamp(points.Length, 20, 100); i++) {
			var x = Random.Range(0, 2 * Mathf.PI);
			var dist = Random.Range(0, 1f);
			Vector3 randomPos = transform.position + new Vector3(Mathf.Sin(x), 0, Mathf.Cos(x)) * dist * range;
			// get nearest point
			var nearestPoint = float.MaxValue;
			if (points.Length > 0) {
				foreach (var p in points) {
					var distToP = (p.position - randomPos).sqrMagnitude;
					if (nearestPoint > distToP) {
						nearestPoint = distToP;
					}
				}
				if (maxDist < nearestPoint) {
					maxDist = nearestPoint;
					maxPos = randomPos;
				}
			} else {
				return randomPos;
			}
		}
		return maxPos;

	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		switch (nodePattern) {
		case NodePatterns.Random:
			Gizmos.DrawWireSphere(transform.position, range);
			break;
		case NodePatterns.DistributedRandom:
			Gizmos.DrawWireSphere(transform.position, range);
			break;
		case NodePatterns.RandomAvoided:
			Gizmos.DrawWireSphere(transform.position, range);
			break;
		case NodePatterns.Grid:
			Gizmos.DrawWireCube(transform.position, new Vector3(range*2, 1, range*2));
			break;
		default:
			break;
		}
	}

}
