using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

[ExecuteInEditMode]
public class NodeHandler : MonoBehaviour
{

	public List<ConnectedNode> nodes = new List<ConnectedNode>();
	public bool initNodesToggle = false;

	public int totalConnectedZones;
	
	// global resource used to store distance between all nodes, and only update nodes when they are moved.
	public Dictionary<ConnectedNode, Dictionary<ConnectedNode, float>> globalDistance = new Dictionary<ConnectedNode,Dictionary<ConnectedNode,float>>();

	public LayerMask obstacleLayerMask;
	public float obstacleRefreshFreq = 1;

	public float connectionRange;
	public bool connectToggle = false;

	void InitNodes()
	{
		// set connection zones, calculate globaldistances
		nodes = GetComponentsInChildren<ConnectedNode>().ToList();
		foreach (var n in nodes) {
			n.handler = this;
			n.ConnectNodes(connectionRange, nodes);
		}
		totalConnectedZones = 1;
		HHHAstar.SetConnectionZones(nodes.Cast<HHHAstar.INode>());
		InitAllDistances(nodes);
		
	}

	public float GetNaiveDistance(ConnectedNode n1, ConnectedNode n2)
	{
		if (n1.GetConnectedZone() == n2.GetConnectedZone()) {
			return (n1.transform.position - n2.transform.position).sqrMagnitude;
		} else {
			return -1;
		}
		
	}

	// used in the hashtable for optimal distance calculations.
	public void InitAllDistances(List<ConnectedNode> nodes)
	{
		globalDistance.Clear();
		foreach (var n1 in nodes) {
			globalDistance.Add(n1, new Dictionary<ConnectedNode,float>());
			foreach (var n2 in nodes) {
				globalDistance[n1].Add(n2, GetNaiveDistance(n1, n2));
			}
		}
	}

	/// <summary>
	/// returns the nearest node to the vector position
	/// </summary>
	public ConnectedNode NearestNode(Vector3 position)
	{

		//var nearestNode = 
		return
			nodes.Aggregate((n1, n2) => {
				return (!n1.Available() ? n2 :
					((n1.transform.position- position).sqrMagnitude < (n2.transform.position- position).sqrMagnitude) ? n1 : n2);
			});

		//return nearestNode;
	}


	void Start()
	{
		InitNodes();
	}


	// Update is called once per frame 
	void Update()
	{
		if (Application.isEditor && !Application.isPlaying) {
			if (initNodesToggle) {
				InitNodes();
				foreach (var n in nodes) {
					UnityEditor.EditorUtility.SetDirty(n);
				}
			}
		} else {

		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		foreach (var node in nodes) {
			foreach (ConnectedNode neighbor in node.neighbors)
			{
				Gizmos.DrawLine(node.transform.position, neighbor.transform.position);
			}
		}
	}
}
