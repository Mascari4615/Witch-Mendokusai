using UnityEngine;

namespace Mascari4615
{
	public abstract class Effect : Artifact
	{
		public abstract void OnEffect();
		public abstract void Cancle();
	}
}