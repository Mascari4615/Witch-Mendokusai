using UnityEngine;

namespace Mascari4615
{
	public class UnlockRecipeEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			DataManager.Instance.HasRecipe[(effectInfo.Data as ItemData).ID] = true;
		}
	}
}