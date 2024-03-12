using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIShop : MonoBehaviour
	{
		[HideInInspector] public List<UIItemSlot> slots = new();
		[SerializeField] private Transform slotsParent;

		private void Awake()
		{
			slots = slotsParent.GetComponentsInChildren<UIItemSlot>().ToList();
			for (int i = 0; i < slots.Count; i++)
				slots[i].SetSlotIndex(i);
		}

		private void OnEnable() => UpdateUI();

		public void UpdateUI()
		{
			var items = SOManager.Instance.ShopKeeperItemBuffer.RuntimeItems;
			for (var i = 0; i < slots.Count; i++)
			{
				if (i < SOManager.Instance.ShopKeeperItemBuffer.RuntimeItems.Count)
				{
					slots[i].SetArtifact(items[i], 1);
					slots[i].gameObject.SetActive(true);
				}
				else
				{
					slots[i].SetArtifact(null);
					slots[i].gameObject.SetActive(false);
				}
			}
		}

		public void BuyItem()
		{

		}
	}
}