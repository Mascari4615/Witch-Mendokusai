using System;
using System.Collections.Generic;

namespace Mascari4615
{
	public class GameEventManager : Singleton<GameEventManager>
	{
		public Dictionary<GameEventType, Action> Callback { get; } = new();

		public void Raise(GameEventType gameEventType)
		{
			if (Callback.TryGetValue(gameEventType, out var action))
			{
				action?.Invoke();
			}
		}

		public void RegisterCallback(GameEventType gameEventType, Action action)
		{
			if (Callback.ContainsKey(gameEventType))
			{
				Callback[gameEventType] += action;
			}
			else
			{
				Callback.Add(gameEventType, action);
			}
		}

		public void UnregisterCallback(GameEventType gameEventType, Action action)
		{
			if (Callback.ContainsKey(gameEventType))
			{
				Callback[gameEventType] -= action;
			}
		}
	}
}