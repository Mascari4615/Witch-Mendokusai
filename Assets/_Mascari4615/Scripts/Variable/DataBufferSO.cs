using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class DataBufferSO<T> : DataSO, ISerializationCallbackReceiver
	{
		[field: SerializeField] public List<T> InitItems { get; private set; }
		[field: NonSerialized] public List<T> Datas { get; protected set; } = new();

		public virtual void Add(T t) => Datas.Add(t);
		public virtual bool Remove(T t) => Datas.Remove(t);
		public virtual void Clear() => Datas.Clear();

		public virtual void OnAfterDeserialize()
		{
			if (InitItems != null && InitItems.Count > 0)
				Datas = InitItems.ToList();
			else
				Datas = new();
		}
		public virtual void OnBeforeSerialize() { }
	}
}