using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class AStarWalker : MonoBehaviour
{
	public NodeHandler nodeHandler;

	public Transform target;
	private Vector3 targetOldPos = new Vector3(0, 0, 0);
	public List<ConnectedNode> curPath = new List<ConnectedNode>();
	public int curNodeOnPath = 0;
	public Vector3 immediateNode;

	/// <summary>
	/// when true, recalculated path does not yet start working, until the next node on the old path is reached.
	/// </summary>
	private bool moving = false;


	public float movementSpeed;

	void Start()
	{
		if (nodeHandler == null) {
			nodeHandler = GameObject.FindObjectOfType<NodeHandler>();
		}
	}

	void Update()
	{
		// move along path
		MoveAlongPath();

		// update path when target moved
		CheckTargetMovement();
	}

	void MoveAlongPath()
	{
		if (curPath != null) {
			if (curNodeOnPath >= curPath.Count) return;
			// only change node when moving == false, a.k.a. when having reached the node that was followed
			if (moving == false) {
				immediateNode = curPath[curNodeOnPath].transform.position;
			}
			// move with fixed speed towards nearest node on curPath
			Vector3 towardsPath = (immediateNode - transform.position);
			Vector3 nextStepDir = towardsPath.normalized * Time.deltaTime * movementSpeed;
			if (immediateNode != transform.position && Vector3.Dot(nextStepDir, immediateNode - transform.position - nextStepDir) >= 0) {
				// move forward only if next step does not exceed position of next node.
				transform.position += nextStepDir;
				moving = true;

			} else {
				curNodeOnPath++;
				moving = false; 
				MoveAlongPath();
				
			}
		}
	}




	// if target moved, calculate path async and follow target.
	void CheckTargetMovement()
	{
		if (target != null) {
			if (target.position != targetOldPos) {
				RecalculatePath();
			}

			targetOldPos = target.position;
		}
	}

	void RecalculatePath()
	{
		var nearestNode = nodeHandler.NearestNode(transform.position);
		curPath = nearestNode.Astar(nodeHandler.NearestNode(target.position));
		if (curPath != null) {
			curPath = curPath.Reverse<ConnectedNode>().ToList();
		}
		curNodeOnPath = 0;
		// if we are already moving on a path, do not change the path until we pass the next node.
		
	}

	void OnDrawGizmos()
	{
		if (curPath != null) {
			if (curPath.Count > curNodeOnPath) {
				if (curPath[curNodeOnPath] != null) {
					Gizmos.color = Color.red;
					// draw line towards direction of movement
					Vector3 towardsPath = (curPath[curNodeOnPath].transform.position - transform.position);
					Vector3 nextStepDir = towardsPath.normalized * Time.deltaTime * movementSpeed;
					Gizmos.DrawRay(transform.position, nextStepDir);
				}
			}
		}
	}

}
