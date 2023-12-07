using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemData), menuName = "Variable/ItemData")]
	public class ItemData : Artifact
	{
		public Grade Grade => grade;
		public ItemType Type => type;
		public Recipe[] Recipes => recipes;
		public int MaxAmount => maxAmount;
		public bool IsCountable => MaxAmount != 1;
		public int PurchasePrice => purchasePrice;
		public int SalePrice => salePrice;

		[SerializeField] private Grade grade;
		[SerializeField] private ItemType type;
		[SerializeField] private Recipe[] recipes;
		[SerializeField] private int maxAmount = 500;
		[SerializeField] private int purchasePrice = 1;
		[SerializeField] private int salePrice = 1;

		public Item CreateItem()
		{
			return new Item(Guid.NewGuid(), this);
		}
	}

	public enum ItemType
	{
		Loot,
		Potion,
		Equipment
	}

	public enum Grade
	{
		Common,
		Uncommon,
		Rare,
		Legendary
	}
}