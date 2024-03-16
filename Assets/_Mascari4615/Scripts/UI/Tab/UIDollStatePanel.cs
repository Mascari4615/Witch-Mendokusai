using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIDollStatePanel : UIPanel
	{
		[SerializeField] private GameObject selectNewEquipmentPanel;
		[SerializeField] private UIItemInventory selectNewEquipmentInventoryUI;

		[SerializeField] private UISlot curDollSlot;
		[SerializeField] private UISlot[] curStuffsSlot;
		private int targetEquipmentIndex;

		public void PrevDoll()
		{
			SetDoll(DataManager.Instance.CurGameData.lastDollIndex - 1);
		}

		public void NextDoll()
		{
			SetDoll(DataManager.Instance.CurGameData.lastDollIndex + 1);
		}

		private void SetDoll(int newDollIndex)
		{
			if (newDollIndex < 0)
				newDollIndex = SOManager.Instance.Dolls.Length - 1;
			else if (newDollIndex == SOManager.Instance.Dolls.Length)
				newDollIndex = 0;

			DataManager.Instance.CurGameData.lastDollIndex = newDollIndex;
			UpdateUI();
		}

		public override void Init()
		{
			selectNewEquipmentPanel.SetActive(false);

			for (int i = 0; i < curStuffsSlot.Length; i++)
			{
				curStuffsSlot[i].SetSlotIndex(i);
				curStuffsSlot[i].SetSelectAction((UISlot slot) =>
				{
					OpenChangeEuqipmentPanel(slot.Index);
				});
			}

			selectNewEquipmentInventoryUI.Init();

			for (int i = 0; i < selectNewEquipmentInventoryUI.Slots.Count; i++)
			{
				selectNewEquipmentInventoryUI.Slots[i].SetSelectAction((UISlot slot) =>
				{
					ApplyNewArtifact(slot.Index);
				});
			}
		}

		public override void UpdateUI(int[] someData = null)
		{
			selectNewEquipmentPanel.SetActive(false);

			if (DataManager.Instance.CurGameData == null)
				return;

			curDollSlot.SetArtifact(DataManager.Instance.DollDic[DataManager.Instance.CurGameData.lastDollIndex]);
			
			for (int i = 0; i < curStuffsSlot.Length; i++)
				curStuffsSlot[i].SetArtifact(DataManager.Instance.GetEquipment(i));
		}

		private void OpenChangeEuqipmentPanel(int equipmentIndex)
		{
			targetEquipmentIndex = equipmentIndex;
			selectNewEquipmentPanel.SetActive(true);
		}

		private void ApplyNewArtifact(int slotIndex)
		{
			// Debug.Log("ApplyNewArtifact" + slotIndex);

			MyDollData[] myDollDatas = DataManager.Instance.CurGameData.myDollDatas;
			int curDollIndex = DataManager.Instance.CurGameData.lastDollIndex;

			Item newEquipment = SOManager.Instance.ItemInventory.GetItem(slotIndex);
			Guid? newEquipmentGUID = null;

			// 빈 슬롯을 선택한 경우
			if (newEquipment == null)
			{
				// Debug.Log("빈 슬롯 선택");
			}
			else
			{
				// Debug.Log("유효한 슬롯 선택");
				newEquipmentGUID = newEquipment.Guid;

				for (int targetDollIndex = 0; targetDollIndex < myDollDatas.Length; targetDollIndex++)
				{
					for (int ei = 0; ei < myDollDatas[targetDollIndex].equipmentGuids.Length; ei++)
					{
						if (myDollDatas[targetDollIndex].equipmentGuids[ei] == newEquipmentGUID)
						{
							if (targetDollIndex == curDollIndex)
							{
								// Debug.Log("이미 이 인형이 끼고 있어요");
								return;
							}
							else
							{
								// Debug.Log("다른 인형이 끼고 있어요");
								myDollDatas[targetDollIndex].equipmentGuids[ei] = myDollDatas[curDollIndex].equipmentGuids[targetEquipmentIndex];
								break;
							}
						}
					}
				}
			}
			myDollDatas[curDollIndex].equipmentGuids[targetEquipmentIndex] = newEquipmentGUID;
			UpdateUI();
		}
	}
}