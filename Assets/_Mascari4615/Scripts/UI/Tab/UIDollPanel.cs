using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIDollPanel : UIPanel
	{
		[SerializeField] private GameObject selectNewEquipmentPanel;
		private UIDollInventory dollInventoryUI;
		private UIItemInventory selectNewEquipmentInventoryUI;
		[SerializeField] private GameObject selectDollButton;
		[SerializeField] private UISlot[] curStuffsSlot;
		private int targetEquipmentIndex;

		// public void PrevDoll() => SetDoll(DataManager.Instance.CurGameData.curDollIndex - 1);
		// public void NextDoll() => SetDoll(DataManager.Instance.CurGameData.curDollIndex + 1);
		// private void SetDoll(int newDollIndex)
		// {
		// 	if (newDollIndex < 0)
		// 		newDollIndex = SOManager.Instance.Dolls.Length - 1;
		// 	else if (newDollIndex == SOManager.Instance.Dolls.Length)
		// 		newDollIndex = 0;

		// 	DataManager.Instance.CurGameData.curDollIndex = newDollIndex;
		// 	UpdateUI();
		// }

		public void SetDoll()
		{
			DataManager.Instance.CurGameData.curDollIndex = dollInventoryUI.CurSlotIndex;
			UpdateUI();
		}

		public override void Init()
		{
			dollInventoryUI = GetComponentInChildren<UIDollInventory>(true);
			selectNewEquipmentInventoryUI = selectNewEquipmentPanel.GetComponentInChildren<UIItemInventory>(true);
			
			selectNewEquipmentPanel.SetActive(false);

			for (int i = 0; i < curStuffsSlot.Length; i++)
			{
				curStuffsSlot[i].SetSlotIndex(i);
				curStuffsSlot[i].SetSelectAction((slot) =>{OpenChangeEuqipmentPanel(slot.Index);});
			}

			dollInventoryUI.Init();
			selectNewEquipmentInventoryUI.Init();

			foreach (UISlot slot in dollInventoryUI.Slots)
			{
				slot.SetSelectAction((slot) =>
				{
					dollInventoryUI.SelectSlot(slot.Index);
					UpdateUI();
				});
			}

			for (int i = 0; i < selectNewEquipmentInventoryUI.Slots.Count; i++)
			{
				selectNewEquipmentInventoryUI.Slots[i].SetSelectAction((slot) =>
				{
					selectNewEquipmentInventoryUI.SelectSlot(slot.Index);
					ApplyNewArtifact(slot.Index);
				});
			}
		}

		public override void UpdateUI()
		{
			int curDollIndex = dollInventoryUI.CurSlotIndex;

			selectNewEquipmentPanel.SetActive(false);

			if (DataManager.Instance.CurGameData == null)
				return;

			if (SOManager.Instance.Dolls.RuntimeItems[curDollIndex].ID == Doll.DUMMY_ID)
			{
				curStuffsSlot[0].transform.parent.gameObject.SetActive(false);
				selectDollButton.SetActive(false);
			}
			else
			{
				curStuffsSlot[0].transform.parent.gameObject.SetActive(true);
				selectDollButton.SetActive(true);
				for (int i = 0; i < curStuffsSlot.Length; i++)
				{
					curStuffsSlot[i].SetArtifact(DataManager.Instance.GetEquipment(curDollIndex, i));
				}
			}
		}

		private void OpenChangeEuqipmentPanel(int equipmentIndex)
		{
			targetEquipmentIndex = equipmentIndex;
			selectNewEquipmentPanel.SetActive(true);
		}

		private void ApplyNewArtifact(int slotIndex)
		{
			// Debug.Log("ApplyNewArtifact" + slotIndex);
			int curDollIndex = dollInventoryUI.CurSlotIndex;

			DollData[] dollDatas = DataManager.Instance.CurGameData.dollDatas;
			// int curDollIndex = DataManager.Instance.CurGameData.lastDollIndex;

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

				for (int targetDollIndex = 0; targetDollIndex < dollDatas.Length; targetDollIndex++)
				{
					for (int ei = 0; ei < dollDatas[targetDollIndex].equipmentGuids.Length; ei++)
					{
						if (dollDatas[targetDollIndex].equipmentGuids[ei] == newEquipmentGUID)
						{
							if (targetDollIndex == curDollIndex)
							{
								// Debug.Log("이미 이 인형이 끼고 있어요");
								dollDatas[curDollIndex].equipmentGuids[ei] = dollDatas[curDollIndex].equipmentGuids[targetEquipmentIndex];
								goto BREAK;
							}
							else
							{
								// Debug.Log("다른 인형이 끼고 있어요");
								dollDatas[targetDollIndex].equipmentGuids[ei] = dollDatas[curDollIndex].equipmentGuids[targetEquipmentIndex];
								goto BREAK;
							}
						}
					}
				}
			}

			BREAK:
			dollDatas[curDollIndex].equipmentGuids[targetEquipmentIndex] = newEquipmentGUID;
			UpdateUI();
		}
	}
}