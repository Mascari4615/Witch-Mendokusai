using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[Serializable]
	public struct Recipe
	{
		public List<ItemData> Ingredients;
		public float Percentage;
	}
}