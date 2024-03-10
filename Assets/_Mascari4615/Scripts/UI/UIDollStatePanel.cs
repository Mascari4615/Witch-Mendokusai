using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIDollStatePanel : MonoBehaviour
	{
		[SerializeField] private GameObject changeEquipmentPanel;
		[SerializeField] private Inventory equipmentInventory;
		[SerializeField] private UISlot curDollSlot;
		[SerializeField] private UISlot[] curStuffsSlot;
		private int changeTargetStuffIndex;

		private void OnEnable()
		{
			UpdateUI();
		}

		public void ChangeDoll(bool turnRight)
		{
			int newDollIndex = DataManager.Instance.CurGameData.lastDollIndex;
			newDollIndex += (turnRight ? 1 : -1);

			if (newDollIndex < 0)
				newDollIndex = SOManager.Instance.Dolls.Length - 1;
			else if (newDollIndex == SOManager.Instance.Dolls.Length)
				newDollIndex = 0;

			DataManager.Instance.CurGameData.lastDollIndex = newDollIndex;
			UpdateUI();
		}

		private void UpdateUI()
		{
			if (DataManager.Instance.CurGameData == null)
				return;

			curDollSlot.SetArtifact(DataManager.Instance.CurDoll);
			for (int i = 0; i < curStuffsSlot.Length; i++)
				curStuffsSlot[i].SetArtifact(DataManager.Instance.GetEquipment(i));
		}

		public void ChangeStuff(int index)
		{
			// OpenChangeEquipmentPanel
			changeEquipmentPanel.gameObject.SetActive(true);
			changeTargetStuffIndex = index;
		}

		public void ApplyNewArtifact(int slotIndex)
		{
			Guid targetEquipmentGUID = equipmentInventory.GetItem(slotIndex).Guid;
			for (int i = 0; i < DataManager.Instance.CurGameData.myDollDatas[0].equipmentGuids.Length; i++)
			{
				if (DataManager.Instance.CurGameData.myDollDatas[0].equipmentGuids[i] == targetEquipmentGUID)
				{
					if (i == slotIndex)
					{
						Debug.Log("ASD");
						return;
					}
					else
					{
						DataManager.Instance.CurGameData.myDollDatas[0].equipmentGuids = null;
						break;
					}
				}
			}

			DataManager.Instance.CurGameData.myDollDatas[0].equipmentGuids[changeTargetStuffIndex] = targetEquipmentGUID;
			changeEquipmentPanel.gameObject.SetActive(false);
			UpdateUI();
		}
	}
}