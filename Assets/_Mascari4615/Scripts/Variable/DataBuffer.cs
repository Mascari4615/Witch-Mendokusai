using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class DataBuffer<T> : ScriptableObject, ISerializationCallbackReceiver
	{
		[field: SerializeField] public List<T> InitItems { get; private set; }
		[field: NonSerialized] public List<T> RuntimeItems { get; protected set; } = new();

		public virtual void Add(T t) => RuntimeItems.Add(t);
		public virtual bool Remove(T t) => RuntimeItems.Remove(t);
		public virtual void Clear() => RuntimeItems.Clear();

		public virtual void OnAfterDeserialize()
		{
			if (InitItems != null && InitItems.Count > 0)
				RuntimeItems = InitItems.ToList();
			else
				RuntimeItems = new();
		}
		public virtual void OnBeforeSerialize() { }
	}
}