using System;
using System.Collections.Generic;
using UnityEngine;

namespace Karmotrine.Script
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameSystem/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        [System.NonSerialized] private readonly List<GameEventListener> _listeners = new();
        private event Action Callback;
        private event Action<Transform> CallbackTransform;

        public void Raise()
        {
            for (var i = _listeners.Count - 1; i >= 0; i--) { _listeners[i].OnEventRaised(); }
            Callback?.Invoke();
        }

        public void Raise(Transform transform)
        {
            for (var i = _listeners.Count - 1; i >= 0; i--) { _listeners[i].OnEventRaised(); }
            CallbackTransform?.Invoke(transform);
        }

        public void RegisterListener(GameEventListener listener) { _listeners.Add(listener); }
        public void UnregisterListener(GameEventListener listener) { _listeners.Remove(listener); }
        public void AddCallback(Action a) { Callback += a; }
        public void RemoveCallback(Action a) { Callback -= a; }
    }
}