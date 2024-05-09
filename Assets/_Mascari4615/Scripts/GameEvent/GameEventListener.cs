using UnityEngine;
using UnityEngine.Events;

namespace Mascari4615
{
	public class GameEventListener : MonoBehaviour
	{
		// public GameEvent Event;
		public GameEventType EventType;
		public UnityEvent Response;

		private void OnEnable()
		{
			// Event.RegisterListener(this);
			GameEventManager.Instance.RegisterCallback(EventType, OnEventRaised);
		}

		private void OnDisable()
		{
			// Event.UnregisterListener(this);
			GameEventManager.Instance.UnregisterCallback(EventType, OnEventRaised);
		}

		public void OnEventRaised()
		{
			// Debug.Log($"{name} : OnEventRaised");
			Response.Invoke();
			// Debug.Log($"{name} : OnEventRaisedEnd");
		}
	}
}