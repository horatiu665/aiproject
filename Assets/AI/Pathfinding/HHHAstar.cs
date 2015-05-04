using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;

public class HHHAstar
{
	public interface INode
	{
		// true when node can be navigated on, false when not. 
		bool Available();

		/// <summary>
		/// returns connected zone ID -> optimize search case where the two nodes are in separate graphs. 
		/// => the search abruptly ends when a node should be searched for in another graph.
		/// </summary>
		int GetConnectedZone();
		void SetConnectedZone(int zone);

		List<INode> GetNeighbors();

		// returns exact distance to other node if neighbors, or estimate if not.
		float CostTo(INode otherNode);

	}

	/// <summary>
	/// sets connection zones, for use in optimization when search should be canceled because nodes belong to different graphs.
	/// </summary>
	/// <param name="allNodes">list of all nodes on which to set connection zones</param>
	public static void SetConnectionZones(IEnumerable<INode> allNodes)
	{
		Hashtable burned = new Hashtable(allNodes.Count());

		int zone = 1;

		foreach (var n in allNodes) {
			// if not checked
			var graphSize = SetConnectionZonesCheckNode(zone, n, allNodes, burned, 0);
			// after this is finished, all blob components were burned.
			if (graphSize > 0) zone++;
		}

	}

	private static int SetConnectionZonesCheckNode(int zone, INode node, IEnumerable<INode> allNodes, Hashtable burned, int graphSize)
	{
		if (burned[node] == null) {
			burned[node] = zone;
			node.SetConnectedZone(zone);

			graphSize++;
			foreach (var neighbor in node.GetNeighbors()) {
				if (neighbor.Available()) {
					graphSize += SetConnectionZonesCheckNode(zone, neighbor, allNodes, burned, 0);
				}
			}
		}

		return graphSize;
	}


	// algorithm. returns path.
	public static List<INode> AStar(INode start, INode goal)
	{
		// nodes evaluated
		List<INode> closed = new List<INode>();

		// nodes to evaluate
		List<INode> open = new List<INode>();

		// nodes forming the path so far
		Hashtable cameFrom = new Hashtable();

		// known costs
		Hashtable knownCosts = new Hashtable();

		// estimated costs
		Hashtable estCosts = new Hashtable();


		// algorithm starts here
		open.Add(start);
		knownCosts[start] = 0f;
		estCosts[start] = (float)knownCosts[start] + start.CostTo(goal);

		while (open.Count > 0) {
			// current is open node with minimum est cost.
			var current = open.Aggregate((n1, n2) => {
				return (float)estCosts[n1] > (float)estCosts[n2] ? n2 : n1;
			});

			if (current == goal)
				return ReconstructPath(cameFrom, goal);

			open.Remove(current);
			closed.Add(current);

			foreach (var neighbor in current.GetNeighbors()) {
				if (!closed.Contains(neighbor) && neighbor.Available()) {
					var tentativeGScore = (float)knownCosts[current] + current.CostTo(neighbor);

					if (!open.Contains(neighbor) || (knownCosts[neighbor] != null && tentativeGScore < (float)knownCosts[neighbor])) {
						cameFrom[neighbor] = current;
						knownCosts[neighbor] = tentativeGScore;
						estCosts[neighbor] = (float)knownCosts[neighbor] + neighbor.CostTo(goal);
						if (!open.Contains(neighbor))
							open.Add(neighbor);
					}

				}
			}
		}
		return null;
	}

	/// <summary>
	/// returns a list of nodes through which, in order, we can go from start to goal.
	/// </summary>
	/// <param name="cameFrom">hashtable of nodes gone through when searching for the path</param>
	/// <param name="current">last node reached when searching for the path</param>
	private static List<INode> ReconstructPath(Hashtable cameFrom, INode current)
	{
		List<INode> totalPath = new List<INode>();
		totalPath.Add(current);
		while (cameFrom.Contains(current)) {
			current = (INode)cameFrom[current];
			totalPath.Add(current);
		}
		return totalPath;
	}

	#region pseudocode A*
	// A*(start, goal)
	// closedset = empty set of nodes already evaluated
	// openset = start; // set of tentative nodes to be evaluated, initially start node
	// came_from = empty map // map of navigated nodes
	// g_score[start] = 0 // cost from start along best known path
	// f_score[start] = g_score[start] + heuristic_cost_estimate(start, goal)
	// while (openset is not empty)
	// current = the node in openset having the lowest f_score[] value
	// if current = goal
	// return reconstruct_path(came_from, goal)
	// remove current from openset
	// add current to closedset
	// for each neighbor in neighbor_nodes(current)
	// if neighbor in closedset
	// continue
	// tentative_g_score = g_score[current] + dist_between(current, neighbor)
	// if neighbor not in openset or tentative_g_score < g_score[neighbor]
	// came_from[neighbor] = current
	// g_score[neighbor] = tentative_g_score;
	// f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal)
	// if neighbor not in openset
	// add neighbor to openset
	// return failure
	// function reconstruct_path (came_from, current)
	// total_path = [current]
	// while current in came_from
	// current = came_from[current]
	// totalpath.append(current)
	// return total_path
	// function heuristic_cost_estimate(start, goal)
	// any function that estimates the cost between start and goal
	// such as linear distance
	// or a curve or other functions.
	#endregion


}
