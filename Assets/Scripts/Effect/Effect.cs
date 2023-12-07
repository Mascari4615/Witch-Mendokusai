using UnityEngine;

namespace Mascari4615
{
	public abstract class Effect : Artifact
	{
		public abstract void OnEquip();
		public abstract void OnRemove();
	}
}