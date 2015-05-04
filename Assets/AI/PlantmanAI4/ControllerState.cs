using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlantmanAI4
{

	public class ControllerState : MonoBehaviour, PlantmanAI4.IState
	{
		public List<IState> states = new List<IState>();
		public IState currentState;

		bool InitStates()
		{
			// sets initial state, etc
			var validStates = states.Where(s => s.ConditionsMet());
			if (validStates.Count() > 0)
				currentState = validStates.Aggregate((s1, s2) =>
					s1.GetPriority() < s2.GetPriority() ? s2 : s1);
			if (currentState == null)
				return false;
			currentState.OnEnter();
			return true;
		}

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

		public string name = "Controller State";
		public float priority;

		public string GetName()
		{
			return name;
		}

		public bool GetUninterruptible()
		{
			return currentState.GetUninterruptible();
		}

		public float GetPriority()
		{
			return priority;
		}

		public void OnEnter()
		{
			InitStates();
		}

		public void OnExecute(float deltaTime)
		{
			UpdateStates(deltaTime);
		}

		public void OnExit()
		{
			currentState.OnExit();
			currentState = null;
		}

		public bool ConditionsMet()
		{
			return states.Any(s => s.ConditionsMet());
		}

		// if this is not part of another state machine, it should run based on unity's events
		public bool runIndependently = false;


		// unity messages

		void Start()
		{
			if (runIndependently) {
				InitStates();
			}
		}

		void Update()
		{
			if (runIndependently) {
				UpdateStates(Time.deltaTime);
			}

		}

	}
}