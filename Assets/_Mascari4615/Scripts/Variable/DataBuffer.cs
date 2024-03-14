using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class DataBuffer<T> : ScriptableObject, ISerializationCallbackReceiver
	{
		public List<T> InitItems;
		[field: NonSerialized] public List<T> RuntimeItems { get; private set; } = new();

		public virtual void AddItem(T t)
		{
			// if (!Items.Contains(t))
			{
				RuntimeItems.Add(t);
			}
		}

		public virtual void RemoveItem(T t)
		{
			if (RuntimeItems.Contains(t))
			{
				RuntimeItems.Remove(t);
			}
		}

		public virtual void ClearBuffer()
		{
			if (RuntimeItems == null)
				return;

			for (int i = RuntimeItems.Count - 1; i >= 0; i--)
				RemoveItem(RuntimeItems[i]);
		}

		public void OnAfterDeserialize()
		{
			if (InitItems != null && InitItems.Count > 0)
				RuntimeItems = InitItems.ToList();
			else
				RuntimeItems = new();
		}
		public void OnBeforeSerialize() { }
	}
}