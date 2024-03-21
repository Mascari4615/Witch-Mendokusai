using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Criteria : ScriptableObject
	{
		public float Progress { get; protected set; }
		public abstract bool IsSatisfied();
		public abstract float GetProgress();
	}
}