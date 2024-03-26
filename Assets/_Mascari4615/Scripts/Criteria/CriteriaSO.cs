using System;
using UnityEngine;

namespace Mascari4615
{
	public abstract class CriteriaSO : ScriptableObject, ICriteria, ISerializationCallbackReceiver
	{
		[field: NonSerialized] public Criteria Data { get; protected set; }

		public virtual float GetProgress()
		{
			return Data.GetProgress();
		}

		public virtual bool Evaluate()
		{
			return Data.Evaluate();
		}

		public abstract void OnAfterDeserialize();
		public virtual void OnBeforeSerialize() {}
	}
}