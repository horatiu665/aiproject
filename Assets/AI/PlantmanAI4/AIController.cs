using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlantmanAI4
{
	public class AIController : MonoBehaviour
	{
		public List<IState> states = new List<IState>();
		public IState currentState;

		bool InitStates()
		{
			// sets initial state, etc
			var validStates = states.Where(s => s.ConditionsMet());
			if (validStates.Count() > 0)
				currentState = validStates.Aggregate((s1, s2)
					=> s1.GetPriority() < s2.GetPriority() ? s2 : s1);
			if (currentState == null)
				return false;
			currentState.OnEnter();
			return true;
		}

		/// <summary>
		/// updates and executes states. can happen once in a while or every frame or whatever.
		/// needs deltaTime for OnExecute()
		/// </summary>
		void UpdateStates(float deltaTime)
		{
			if (currentState == null) {
				if (!InitStates()) {
					return;
				}
			}

			if (!currentState.GetUninterruptible()) {
				// takes list of valid states, chooses highest priority. 
				var validStates = states.Where(s => s.ConditionsMet());
				var highestPriorityState = validStates.Aggregate((s1, s2)
					=> s1.GetPriority() < s2.GetPriority() ? s2 : s1);

				// if different than current state, switch to it.
				if (highestPriorityState != currentState) {
					currentState.OnExit();
					currentState = highestPriorityState;
					currentState.OnEnter();
				}
			}

			currentState.OnExecute(deltaTime);

		}

		void Start()
		{
			InitStates();

		}

		void Update()
		{
			UpdateStates(Time.deltaTime);

		}

	}
}