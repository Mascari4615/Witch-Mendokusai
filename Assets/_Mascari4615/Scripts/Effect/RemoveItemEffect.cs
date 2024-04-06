using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(RemoveItemEffect), menuName = "Effect/" + nameof(RemoveItemEffect))]
	public class RemoveItemEffect : Effect
	{
		[SerializeField] private ItemData targetItem;
		[SerializeField] private int amount;

		public override void Apply()
		{
			SOManager.Instance.ItemInventory.Remove(SOManager.Instance.ItemInventory.FindItemIndex(targetItem), amount);
		}

		public override void Cancle()
		{
		}
	}
}