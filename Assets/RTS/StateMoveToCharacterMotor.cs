using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RotationModule))]
public class StateMoveToCharacterMotor : MonoBehaviour, PlantmanAI4.IState {

	// destination
	public Vector3 targetPosition;

	// units per second
	public float movementSpeed = 10;

	private CharacterMotor characterMotor;
	private RotationModule rotationModule;

	public event System.EventHandler Exit;

	void Start()
	{
		characterMotor = GetComponent<CharacterMotor>();
		rotationModule = GetComponent<RotationModule>();

	}

	public string GetName()
	{
		return "Move To CharacterMotor " + targetPosition;
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
		Vector3 dir = targetPosition - transform.position;
		characterMotor.inputMoveDirection = dir.normalized * movementSpeed;
		rotationModule.RotateTowards(dir);
		if (closeEnoughToTarget()) {
			OnExit();
		}
	}

	private bool closeEnoughToTarget()
	{
		var tar = targetPosition;
		tar.y = 0;
		var cur = transform.position;
		cur.y = 0;
		return (tar - cur).sqrMagnitude <= 0.2f;
	}

	public void OnExit()
	{
		characterMotor.inputMoveDirection = Vector3.zero;
		Exit(this, null);
	}

	public bool ConditionsMet()
	{
		return true;
	}
}
