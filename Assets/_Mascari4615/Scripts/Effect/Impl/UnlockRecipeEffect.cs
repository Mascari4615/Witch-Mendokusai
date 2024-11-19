using UnityEngine;

namespace Mascari4615
{
	public class UnlockRecipeEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			DataManager.Instance.IsRecipeUnlocked[(effectInfo.Data as ItemData).ID] = true;
		}
	}
}