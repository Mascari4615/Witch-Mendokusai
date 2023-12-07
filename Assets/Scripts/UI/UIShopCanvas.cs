using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
    public class UIShopCanvas : MonoBehaviour
    {
		[HideInInspector] public List<ItemSlot> slots = new();
		[SerializeField] private Transform slotsParent;

		private void Awake()
		{
			slots = slotsParent.GetComponentsInChildren<ItemSlot>().ToList();
			for (int i = 0; i < slots.Count; i++)
				slots[i].SetSlotIndex(i);
		}

		private void OnEnable() => UpdateUI();

		public void UpdateUI()
		{
			var items = DataManager.Instance.shopKeeperItemBuffer.RuntimeItems;
			for (var i = 0; i < slots.Count; i++)
			{
				if (i < DataManager.Instance.shopKeeperItemBuffer.RuntimeItems.Count)
				{
					slots[i].UpdateUI(items[i], 1);
					slots[i].gameObject.SetActive(true);
				}
				else
				{
					slots[i].UpdateUI(null);
					slots[i].gameObject.SetActive(false);
				}
			}
		}

		public void BuyItem()
		{

		}
	}
}