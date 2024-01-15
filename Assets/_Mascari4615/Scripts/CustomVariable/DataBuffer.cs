using System.Collections.Generic;
using System.Linq;
using PlayFab.EconomyModels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class DataBuffer<T> : ScriptableObject
	{
		public List<T> InitItems;
		[field: System.NonSerialized] public List<T> RuntimeItems { get; private set; } = new();

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

		public void OnAfterDeserialize() { RuntimeItems = InitItems.ToList(); }
	}
}