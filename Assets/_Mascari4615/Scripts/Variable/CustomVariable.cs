using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class CustomVariable<T> : Artifact, ISerializationCallbackReceiver
	{
		[field: Header("_" + nameof(CustomVariable<T>))]
		[field: SerializeField] public T InitialValue { get; private set; }
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

		public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
		public void OnBeforeSerialize() { }
	}
}