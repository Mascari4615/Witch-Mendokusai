using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemData), menuName = "Variable/ItemData")]
	public class ItemData : DataSO
	{
		[field: Header("_" + nameof(ItemData))]
		[PropertyOrder(10)][field: SerializeField] public ItemGrade Grade { get; private set; }
		[PropertyOrder(11)][field: SerializeField] public ItemType Type { get; private set; }
		[PropertyOrder(12)][field: SerializeField] public Recipe[] Recipes { get; private set; }
		[PropertyOrder(13)][field: SerializeField] public int MaxAmount { get; private set; } = 500;
		[PropertyOrder(14)][field: SerializeField] public int PurchasePrice { get; private set; }
		[PropertyOrder(15)][field: SerializeField] public int SalePrice { get; private set; }

		public bool IsCountable => MaxAmount != 1;

		public Item CreateItem()
		{
			return new Item(Guid.NewGuid(), this);
		}
	}
}