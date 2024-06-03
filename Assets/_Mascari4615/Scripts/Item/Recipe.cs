using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[Serializable]
	public struct Recipe
	{
		public RecipeType Type;
		public List<ItemData> Ingredients;
		public int priceNyang;
		public float Percentage;

		public List<RewardInfo> FailureRewards;
		public List<RewardInfo> SuccessRewards;
	}
}