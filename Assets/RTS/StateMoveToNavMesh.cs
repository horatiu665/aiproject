using UnityEngine;
using System.Collections;

public class StateMoveToNavMesh : MonoBehaviour, PlantmanAI4.IState
{

	// destination
	public Vector3 targetPosition;

	// units per second
	public float movementSpeed = 10;

	private NavMeshAgent agent;
	
	public event System.EventHandler Exit;

	void Start()
	{
		agent = GetComponentInParent<NavMeshAgent>();
	}

	public string GetName()
	{
		return "Move To NavMesh " + targetPosition;
	}

	public bool GetUninterruptible()
	{
		return false;
	}

	public float GetPriority()
	{
		return 1;
	}

	public void OnEnter()
	{
		
	}

	public void OnExecute(float deltaTime)
	{
		agent.SetDestination(targetPosition);

		if (closeEnoughToTarget()) {
			OnExit();
		}
	}

	private bool closeEnoughToTarget()
	{
		return agent.PathCompleted();
	}

	public void OnExit()
	{
		Exit(this, null);
	}

	public bool ConditionsMet()
	{
		return true;
	}
}
