using UnityEngine;
using System.Collections;

public static class NavMeshAgentExtensions {

	public static bool PathCompleted(this NavMeshAgent agent)
	{
		// Check if we've reached the destination
		if (!agent.pathPending) {
			if (agent.remainingDistance <= agent.stoppingDistance) {
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
					// Done
					return true;
				}
			}
		}
		return false;
	}
}
