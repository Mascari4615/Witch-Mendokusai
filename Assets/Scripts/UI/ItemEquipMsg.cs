using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

namespace Mascari4615
{
	public class ItemEquipMsg : MonoBehaviour
	{
		[SerializeField] private ItemEquipMsgElement[] equipMsgElements;
		[SerializeField] private ItemVariable lastEquippedItem;

		private int curElementIndex = 0;

		private readonly WaitForSeconds ws01 = new(.1f);
		private readonly Queue<ItemData> toolTipStacks = new();
		public void EquipItem()
		{
			toolTipStacks.Enqueue(lastEquippedItem.RuntimeValue);
			StartCoroutine(ShowToolTip());
		}

		private IEnumerator ShowToolTip()
		{
			while (toolTipStacks.Count > 0)
			{
				var itemData = toolTipStacks.Dequeue();
				equipMsgElements[curElementIndex].SetAndPop(itemData);
				equipMsgElements[curElementIndex].transform.SetAsFirstSibling();
				curElementIndex = (curElementIndex + 1) % equipMsgElements.Length;
				RuntimeManager.PlayOneShot($"event:/SFX/Equip");
				yield return ws01;
			}
		}

		public void StopToolTip()
		{
			StopAllCoroutines();
		}
	}
}