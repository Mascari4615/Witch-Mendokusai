using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class ItemObject : LootObject
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		private ItemData itemData;

		public void Init(ItemData itemData)
		{
			this.itemData = itemData;
			spriteRenderer.sprite = itemData.Sprite;
		}

		public override void Effect()
		{
			RuntimeManager.PlayOneShot("event:/SFX/Loot", transform.position);
			SOManager.Instance.ItemInventory.Add(itemData);
		}
	}
}