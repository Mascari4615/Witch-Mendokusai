using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class PickupObject : MonoBehaviour, IInteractable
	{
		[SerializeField] private ItemData itemData;
		[SerializeField] private int amount;

		public void OnInteract()
		{
			SOManager.Instance.ItemInventory.Add(itemData, amount);
			gameObject.SetActive(false);
		}
	}
}