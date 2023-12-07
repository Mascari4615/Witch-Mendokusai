using System.Collections.Generic;
using System.Linq;
using PlayFab.EconomyModels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class DataBuffer<T> : ScriptableObject
	{
		[FormerlySerializedAs("items")] public List<T> InitItems;
		[field: System.NonSerialized] public List<T> RuntimeItems { get; private set; } = new();

		public virtual void Add(T t)
		{
			// if (!Items.Contains(t))
			{
				RuntimeItems.Add(t);
			}
		}

		public virtual void Remove(T t)
		{
			if (RuntimeItems.Contains(t))
			{
				RuntimeItems.Remove(t);
			}
		}

		public virtual void Clear()
		{
			if (RuntimeItems == null)
				return;

			for (int i = RuntimeItems.Count - 1; i >= 0; i--)
				Remove(RuntimeItems[i]);
		}

		public void OnAfterDeserialize() { RuntimeItems = InitItems.ToList(); }
	}
}