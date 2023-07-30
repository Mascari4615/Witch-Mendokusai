using Karmotrine.Script;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CustomVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private T initialValue;
    public T RuntimeValue
    {
        get => _runtimeValue;
        set
        {
            _runtimeValue = value;
            gameEvent?.Raise();
        }
    }
    [System.NonSerialized] private T _runtimeValue;
    [SerializeField] private GameEvent gameEvent;

    public void OnAfterDeserialize() { RuntimeValue = initialValue; }
    public void OnBeforeSerialize() { }
}