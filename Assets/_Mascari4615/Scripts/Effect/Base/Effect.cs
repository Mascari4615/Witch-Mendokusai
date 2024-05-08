using UnityEngine;

namespace Mascari4615
{
	public abstract class Effect : DataSO
	{
		public abstract void Apply();
		public abstract void Cancle();
	}
}