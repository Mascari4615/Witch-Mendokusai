using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using MoreMountains.Feedbacks;

namespace Mascari4615
{
	public class UItemEquipPopup : MonoBehaviour
	{
		private UISlot[] uiSlots;
		private MMF_Player[] mmfPlayers;

		private int curElementIndex = 0;

		private readonly WaitForSecondsRealtime ws01 = new(.1f);
		private readonly Queue<ItemData> toolTipStacks = new();
	
		private void Awake()
		{
			uiSlots = GetComponentsInChildren<UISlot>(true);
			mmfPlayers = GetComponentsInChildren<MMF_Player>(true);

			SOManager.Instance.LastEquipedItem.GameEvent.AddCallback(EquipItem);
		}
		
		public void EquipItem()
		{
			toolTipStacks.Enqueue(SOManager.Instance.LastEquipedItem.RuntimeValue);
			StartCoroutine(ShowToolTip());
		}

		private IEnumerator ShowToolTip()
		{
			while (toolTipStacks.Count > 0)
			{
				ItemData itemData = toolTipStacks.Dequeue();
				
				uiSlots[curElementIndex].SetArtifact(itemData);
				uiSlots[curElementIndex].transform.SetAsFirstSibling();
				mmfPlayers[curElementIndex].StopFeedbacks();
				mmfPlayers[curElementIndex].PlayFeedbacks();
				
				curElementIndex = (curElementIndex + 1) % uiSlots.Length;
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