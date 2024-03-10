using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class CustomVariable<T> : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField] private T initialValue;
		public T RuntimeValue
		{
			get => _runtimeValue;
			set
			{
				_runtimeValue = value;
				GameEvent?.Raise();
			}
		}
		[System.NonSerialized] private T _runtimeValue;
		[field: SerializeField] public GameEvent GameEvent { get; private set; }

		public void OnAfterDeserialize() { RuntimeValue = initialValue; }
		public void OnBeforeSerialize() { }
	}
}