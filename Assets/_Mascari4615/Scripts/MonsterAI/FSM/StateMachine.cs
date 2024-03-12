using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mascari4615
{
	public enum TempState
	{
		Idle,
		Attack
	}

	public enum StateEvent
	{
		Enter,
		Update,
		Exit
	}

	public abstract class StateMachine<T> : MonoBehaviour where T : Enum
	{
		// private const float TICK = 0.1f;

		protected T currentState;
		private readonly Dictionary<(T, StateEvent), Action> stateEventDic = new();
		private readonly Coroutine coroutine;

		private void OnEnable()
		{
			ChangeState(default);
		}

		private void OnDisable()
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
		}
		
		public void SetStateEvent(T state, StateEvent stateEvent, Action action)
		{
			stateEventDic[(state, stateEvent)] = action;
		}

		public void ChangeState(T newState)
		{
			if (currentState != null)
			{
				if (stateEventDic.ContainsKey((currentState, StateEvent.Exit)))
					stateEventDic[(currentState, StateEvent.Exit)]?.Invoke();
			}
			currentState = newState;
			if (stateEventDic.ContainsKey((currentState, StateEvent.Enter)))
				stateEventDic[(currentState, StateEvent.Enter)]?.Invoke();

			if (coroutine != null)
				StopCoroutine(coroutine);
			StartCoroutine(StateLoop());
		}

		private IEnumerator StateLoop()
		{
			// WaitForSeconds waitForTick = new(TICK);
			while (true)
			{
				stateEventDic[(currentState, StateEvent.Update)]?.Invoke();
				// yield return waitForTick;
				yield return null;
			}
		}
	}
}