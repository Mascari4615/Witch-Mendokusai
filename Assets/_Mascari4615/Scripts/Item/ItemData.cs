using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemData), menuName = "Variable/ItemData")]
	public class ItemData : Artifact
	{
		public bool IsCountable => MaxAmount != 1;
		
		[field: SerializeField] public Grade Grade { get; private set; }
		[field: SerializeField] public ItemType Type { get; private set; }
		[field: SerializeField] public Recipe[] Recipes { get; private set; }
		[field: SerializeField] public int MaxAmount  { get; private set; }= 500;
		[field: SerializeField] public int PurchasePrice { get; private set; } = 1;
		[field: SerializeField] public int SalePrice { get; private set; } = 1;

		public Item CreateItem()
		{
			return new Item(Guid.NewGuid(), this);
		}
	}

	public enum ItemType
	{
		None = -1,
		Loot,
		Equipment,
		Potion,
	}

	public enum Grade
	{
		Common,
		Uncommon,
		Rare,
		Legendary
	}
}