using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public Transform[] spawnWhat;
	public int howMany = 1;
	public float spawnRadius = 10;
	[Range(1, 1.999f)]
	public float variousSizes = 1;
	public bool distributedRandom = true;
	public bool randomize;
	public int randomSeed = 0;
	public float delay = 0;
	public Color editorColor;

	void Randomize(int s)
	{
		Random.seed = s;
	}

	// Use this for initialization
	IEnumerator Start()
	{
		if (delay > 0) {
			yield return new WaitForSeconds(delay);
		}

		if (randomize)
			Randomize(randomSeed);

		// spawner
		for (int i = 0; i < howMany; i++) {
			// little more normally distributed random position
			Vector3 pos = distributedRandom
				? (Random.insideUnitCircle + Random.insideUnitCircle) / 2 * spawnRadius
				: Random.insideUnitCircle * spawnRadius;
			pos = new Vector3(transform.position.x + pos.x, transform.position.y, transform.position.z + pos.y);
			Transform a = Instantiate(spawnWhat[Random.Range(0, spawnWhat.Length)], pos, Quaternion.identity) as Transform;
			a.localScale *= Random.Range(2 - variousSizes, variousSizes);
			//a.parent = transform;

		}

	}

	void OnDrawGizmos()
	{
		Gizmos.color = editorColor;
		Gizmos.DrawWireSphere(transform.position, spawnRadius);

	}

	/// <summary>
	/// could be used for finding unique positions for each element, so they are not overlapping terribly. instead, a dirty fix can also be found.
	/// </summary>
	float radiusOfObjects(int numObjects, float circleRadius)
	{
		return circleRadius / Mathf.Sqrt(numObjects);

	}

}
