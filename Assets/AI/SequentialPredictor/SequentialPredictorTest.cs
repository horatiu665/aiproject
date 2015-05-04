using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SequentialPredictorTest : MonoBehaviour {

	List<int> numbers = new List<int>();
	public int historySize = 500;
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown && Input.inputString.Length > 0) {
			numbers.Add(int.Parse(Input.inputString.Substring(0, 1)));
		}
	}

	void OnGUI()
	{
		var sss = "";
		for (int i = 0; i < numbers.Count; i++) {
			sss += numbers[i].ToString() + " ";
		}
		GUI.Label(new Rect(0, 0, Screen.width, 100), sss);

		// alphabet size
		int alpSize = numbers.GroupBy(i => i).Count();

		GUI.Label(new Rect(0, 20, Screen.width, 100), "Next nr is " + SequentialPredictor.PredictNext(numbers, 10, historySize).ToString());
		
	}
}
