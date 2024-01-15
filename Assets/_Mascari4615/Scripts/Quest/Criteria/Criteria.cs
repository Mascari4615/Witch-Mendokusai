using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Criteria : ScriptableObject
	{
		public abstract bool HasComplete();
	}
}