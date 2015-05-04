using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
[System.Serializable]
public class ConnectedNode : MonoBehaviour, HHHAstar.INode
{
	// set by handler when managing nodes.
	public NodeHandler handler;

	// list of neighbors of node, calculated when node moves
	public List<ConnectedNode> neighbors = new List<ConnectedNode>();

	public bool Available() { return nodeAvailable; }

	/// <summary>
	/// when false, node is treated as it would not exist.
	/// </summary>
	public bool nodeAvailable = true;

	// returns list of node neighbors
	public List<HHHAstar.INode> GetNeighbors()
	{
		return neighbors.Cast<HHHAstar.INode>().ToList();
	}

	//private Hashtable costsTo = new Hashtable();

	// returns cost between two nodes via various methods.
	public float CostTo(HHHAstar.INode otherNode)
	{
		return handler.globalDistance[(ConnectedNode)otherNode][this];
		//if (neighbors.Contains((ConnectedNode)otherNode)) {
		//	return (float)costsTo[otherNode];//(((ConnectedNode)otherNode).transform.position - transform.position).sqrMagnitude;
		//} else {
		//	return (((ConnectedNode)otherNode).transform.position - transform.position).sqrMagnitude;
		//}
	}

	// nodes belong to subgraphs, search should not search when there is no path between two nodes.
	#region connected zones
	public int GetConnectedZone()
	{
		return connectionZone;
	}
	public void SetConnectedZone(int zone)
	{
		connectionZone = zone;
		if (handler.totalConnectedZones < zone)
			handler.totalConnectedZones = zone;
	}
	private int connectionZone = 0;
	public static int maxConnectionZone = 1;
	#endregion

	/// <summary>
	/// connects to all nodes within range range
	/// </summary>
	/// <param name="range">range to connect nodes</param>
	public void ConnectNodes(float range, List<ConnectedNode> allNodes, bool connectToEachOther = false)
	{
		neighbors.Clear();
		foreach (var node in allNodes) {
			if (connectToEachOther) {
				// default is removed
				node.neighbors.Remove(this);
			}
			if ((node.transform.position - transform.position).sqrMagnitude < range * range) {
				if (node != this) {
					neighbors.Add(node);

					if (connectToEachOther) {
						if (!node.neighbors.Contains(this)) {
							node.neighbors.Add(this);
						}
					}

				}
			}
		}
	}


	public List<ConnectedNode> Astar(ConnectedNode destination)
	{
		var astarResult = HHHAstar.AStar(this, destination);
		if (astarResult != null) {
			return astarResult.Select(inode => (ConnectedNode)inode).ToList();
		} else
			return null;
	}

	public void SetNodeAvailable()
	{
		// if node is not within colliders marked "obstacle", it can be considered available.
		nodeAvailable = (!Physics.CheckSphere(transform.position, handler.connectionRange * 0.2f, handler.obstacleLayerMask));
	}


	Vector3 oldNodePos;

	void Start()
	{
		oldNodePos = transform.position;
		StartCoroutine(SNACor());
	}

	IEnumerator SNACor()
	{
		yield return new WaitForSeconds(Random.Range(0, 1 / handler.obstacleRefreshFreq));
		while (true) {
			yield return new WaitForSeconds(1 / handler.obstacleRefreshFreq);
			SetNodeAvailable();
		}
	}

	/// if node moves, update it, do stuff.
	void Update()
	{
		if (Application.isPlaying) {
			if (oldNodePos != transform.position) {
				oldNodePos = transform.position;

				// update distances to other nodes
				foreach (var n in handler.nodes) {
					if (n != this) {
						handler.globalDistance[n][this] = handler.globalDistance[this][n] = handler.GetNaiveDistance(this, n);
					}
				}

				// update neighbors
				ConnectNodes(handler.connectionRange, handler.nodes, true);

				// update all connected zones actually...
				HHHAstar.SetConnectionZones(handler.nodes.Cast<HHHAstar.INode>());

			}
		}
	}
}
