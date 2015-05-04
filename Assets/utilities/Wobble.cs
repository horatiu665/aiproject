using UnityEngine;
using System.Collections;

public class Wobble : MonoBehaviour
{

	public Vector3 pos, rot, sca;
	private Vector3		initPos, initRot, initSca;
	private float _t;
	public float positionWobbleSpeed, rotationWobbleSpeed, scaleWobbleSpeed;

	void Start()
	{
		initPos = transform.position;
		initRot = transform.eulerAngles;
		initSca = transform.localScale;

	}

	void Update()
	{
		transform.position -= pos * Mathf.Sin(_t * positionWobbleSpeed);
		transform.eulerAngles -= rot * Mathf.Sin(_t * rotationWobbleSpeed);
		transform.localScale -= sca * Mathf.Sin(_t * scaleWobbleSpeed);
		_t += Time.deltaTime;
		transform.position += pos * Mathf.Sin(_t * positionWobbleSpeed);
		transform.eulerAngles += rot * Mathf.Sin(_t * rotationWobbleSpeed);
		transform.localScale += sca * Mathf.Sin(_t * scaleWobbleSpeed);

	}
}
