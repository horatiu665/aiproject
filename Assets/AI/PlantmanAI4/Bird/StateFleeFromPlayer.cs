using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RotationModule))]
public class StateFleeFromPlayer : MonoBehaviour, PlantmanAI4.IState
{

	public float distanceToFlee = 10;
	public Transform player;
	public float fleeSpeed = 1;

	public float uninterruptibleTime = 3f;
	float uninterruptibleTimer = 0;

	BirdFunc bird;
	private RotationModule rotationModule;

	void Start()
	{
		bird = GetComponent<BirdFunc>();
		rotationModule = GetComponent<RotationModule>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	public string GetName()
	{
		return "Flee from player";
	}

	public bool GetUninterruptible()
	{
		return uninterruptibleTimer > 0;
	}

	public float GetPriority()
	{
		// proximity to player is priority. when player is closer than distance D, larger than zero.
		try {
			return distanceToFlee - (player.position - transform.position).magnitude;
		}
		catch {
			return -1;
		}
	}

	public void OnEnter()
	{
		StartCoroutine(pTween.To(uninterruptibleTime, 1, 0, t => {
			uninterruptibleTimer = t;
		}));
	}

	public void OnExecute(float deltaTime)
	{
		// fle from player
		var playerTowardsTransform = transform.position - player.position;
		rotationModule.RotateTowards(playerTowardsTransform.normalized);
		transform.position += playerTowardsTransform.normalized * deltaTime * fleeSpeed;

		bird.FlyDown(deltaTime);
	}


	public void OnExit()
	{
	}

	public bool ConditionsMet()
	{
		return true;
	}
}
