using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PlantmanAI4
{
	[ExecuteInEditMode]
	public class AIControllerSetStatesOnStart : MonoBehaviour
	{
		public bool setStatesNow = false;
		public List<MonoBehaviour> addOnlyTheseStates = new List<MonoBehaviour>();

		void Start()
		{
			if (addOnlyTheseStates.Any()) {
				var con = GetComponent<ControllerState>();
				con.states = GetComponentsInChildren<IState>().Where(s => s != con).Where(s => addOnlyTheseStates.Any(mb => mb == s)).ToList();
			} else {
				var con = GetComponent<ControllerState>();
				con.states = GetComponentsInChildren<IState>().Where(s => s != con).ToList();
			}
		}

#if UNITY_EDITOR
		void Update()
		{
			if (setStatesNow) {
				setStatesNow = false;
				Start();
			}
		}
#endif

	}
}
