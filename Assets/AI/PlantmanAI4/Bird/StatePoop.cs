using UnityEngine;
using System.Collections;

public class StatePoop : MonoBehaviour, PlantmanAI4.IState {

	public float needForPoop = -10;
	public Transform poop;
	public Vector2 poopRandomTime = new Vector2(5, 15);
	public AnimationCurve shakeIntensity;
	public float shakeIntensityMultiplier = 1;

	void Start()
	{
		needForPoop = Random.Range(-poopRandomTime.y, -poopRandomTime.x);

	}

	void Update()
	{
		needForPoop += Time.deltaTime;
	}



	public string GetName()
	{
		return "Utility Poop";
	}

	public bool GetUninterruptible()
	{
		return false;
	}

	public float GetPriority()
	{
		return needForPoop;
	}

	public void OnEnter()
	{
		// create poop after a little shake
		var initPos= transform.position;
		StartCoroutine(pTween.To(0.7f, t => {
			// shake
			transform.position = initPos + Random.insideUnitSphere * shakeIntensity.Evaluate(t) * shakeIntensityMultiplier;
			if (t == 1) {
				Instantiate(poop, transform.position, transform.rotation);
				needForPoop = Random.Range(-poopRandomTime.y, -poopRandomTime.x);
			}
		}));
	}

	public void OnExecute(float deltaTime)
	{

	}

	public void OnExit()
	{

	}

	public bool ConditionsMet()
	{
		return true;
	}
}
