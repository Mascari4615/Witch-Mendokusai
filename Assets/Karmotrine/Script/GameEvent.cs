using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSystem/GameEvent")]
public class GameEvent : ScriptableObject
{
    [System.NonSerialized] private List<GameEventListener> listeners = new();
    private event Action Callback;
    private event Action<Transform> CallbackTransform;

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--) { listeners[i].OnEventRaised(); }
        if (Callback is not null) Callback.Invoke();
    }

    public void Raise(Transform transform)
    {
        for (int i = listeners.Count - 1; i >= 0; i--) { listeners[i].OnEventRaised(); }
        if (CallbackTransform is not null) CallbackTransform.Invoke(transform);
    }

    public void RegisterListener(GameEventListener listener) { listeners.Add(listener); }
    public void UnregisterListener(GameEventListener listener) { listeners.Remove(listener); }
    public void AddCollback(Action a) { Callback += a; }
    public void RemoveCollback(Action a) { Callback -= a; }
}