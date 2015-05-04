using UnityEngine;
using System.Collections;
using System.Linq;


[RequireComponent(typeof(RotationModule))]
public class StateIdleFly : MonoBehaviour, PlantmanAI4.IState
{
	public Vector3 speed;
	BirdFunc bird;
	private RotationModule rotationModule;

	void Start()
	{
		bird = GetComponent<BirdFunc>();
		rotationModule = GetComponent<RotationModule>();
	}

	public string GetName()
	{
		return "Idle fly";
	}

	public bool GetUninterruptible()
	{
		return false;
	}

	public float GetPriority()
	{
		return 0;
	}

	public void OnEnter()
	{
		// rotate towards wherever is closer to 0,0,0
		if ((transform.position + transform.forward + transform.right).magnitude > (transform.position + transform.forward - transform.right).magnitude) {
			speed.x = - Mathf.Abs(speed.x);
		} else {
			speed.x = Mathf.Abs(speed.x);
		}
		//speed.x = speed.x * (Random.Range(0, 1) * 2 - 1);

	}

	public void OnExecute(float deltaTime)
	{

		var flyDir = transform.forward * speed.z + transform.right * speed.x + transform.up * speed.y;

		var lookdir = flyDir;
		lookdir.y = 0;

		rotationModule.RotateTowards(lookdir);

		// fly in the sky
		transform.position += flyDir * deltaTime;

		bird.FlyDown(deltaTime);
	}

	public void OnExit()
	{

	}

	/// <summary>
	/// idle is always an option
	/// </summary>
	/// <returns></returns>
	public bool ConditionsMet()
	{
		return true;
	}
}
