using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class MasteryManager : MonoBehaviour
	{
		private enum MasteryUIState
		{
			Wait,
			SelectDeck,
			SelectCard,
		}

		private readonly List<List<Mastery>> masteryDataBuffers = new(3) { new(), new(), new() };
		private int curDeckIndex;
		
		[SerializeField] private MasteryDataBuffer selectMasteryDataBuffer;

		[SerializeField] private GameObject selectDeckPanel;
		[SerializeField] private GameObject deckPanel;
		[SerializeField] private UISlot[] deckSelectButtons;
		[SerializeField] private UICardSlot[] masterySelectButtons;
		private readonly Dictionary<int, UIDeck> deckUIDic = new();
		private MasteryUIState curState = MasteryUIState.Wait;

		private void Awake()
		{
			UIDeck[] deckUIs = FindObjectsOfType<UIDeck>(true);
			foreach (UIDeck deckUI in deckUIs)
				deckUIDic.Add(deckUI.EquipmentData.ID, deckUI);

			ChangeState(MasteryUIState.Wait);
		}

		public void Init()
		{
			ChangeState(MasteryUIState.Wait);

			while (selectMasteryDataBuffer.RuntimeItems.Count > 0)
				selectMasteryDataBuffer.RemoveItem(selectMasteryDataBuffer.RuntimeItems[^1]);
			selectMasteryDataBuffer.ClearBuffer();

			foreach (List<Mastery> masteryDataBuffer in masteryDataBuffers)
				masteryDataBuffer.Clear();

			for (int i = 0; i < 3; i++)
			{
				EquipmentData equipment = DataManager.Instance.GetEquipment(i);

				if (equipment == null)
					continue;
				masteryDataBuffers[i].AddRange(equipment.Masteries);

				deckUIDic[equipment.ID].Init(
					cardSelectAction: (UISlot slot) =>
					{
						// SelectMastery(masterySlots[i].Atrifact as Mastery);
						// 원래 위 코드를 썼는데, 클로저 문제로 인해 아래처럼 바꿈

						SelectCard(slot.Artifact as Mastery);
					});

				deckSelectButtons[i].SetSlotIndex(i);
				deckSelectButtons[i].SetArtifact(equipment);
				deckSelectButtons[i].SetSelectAction(
					(UISlot slot) =>
					{
						SelectDeck(slot.Index);
					});

				masterySelectButtons[i].SetSelectAction(
					(UISlot slot) =>
					{
						SelectCard(slot.Artifact as Mastery);
					});
			}
		}

		public void LevelUp()
		{
			TimeManager.Instance.Pause();
			StartCoroutine(LevelUpCoroutine());
		}

		private IEnumerator LevelUpCoroutine()
		{
			yield return new WaitForSecondsRealtime(1f);
			ChangeState(MasteryUIState.SelectDeck);
		}

		public void SelectDeck(int deckIndex)
		{
			curDeckIndex = deckIndex;
			List<Mastery> curDeckBuffer = masteryDataBuffers[deckIndex];

			if (curDeckBuffer.Count < 3)
			{
				Debug.LogError("Not Enough Mastery Count");
				return;
			}

			List<Mastery> randomMasteries = new();
			while (randomMasteries.Count < 3)
			{
				int randomIndex = Random.Range(0, curDeckBuffer.Count);
				Mastery randomMastery = curDeckBuffer[randomIndex];

				if (randomMasteries.Contains(randomMastery))
					continue;

				masterySelectButtons[randomMasteries.Count].SetArtifact(randomMastery);
				randomMasteries.Add(randomMastery);
			}

			ChangeState(MasteryUIState.SelectCard);

			int equipmentID = DataManager.Instance.GetEquipment(deckIndex).ID;
			deckUIDic[equipmentID].UpdateUI(randomMasteries);
			deckUIDic[equipmentID].gameObject.SetActive(true);
		}

		public void SelectCard(Mastery mastery)
		{
			RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);

			List<Mastery> curDeckBuffer = masteryDataBuffers[curDeckIndex];
			selectMasteryDataBuffer.AddItem(mastery);

			int sameMasteryCount = selectMasteryDataBuffer.RuntimeItems.Select(m => m == mastery).Count();
			if (mastery.MaxStack == sameMasteryCount)
			{
				int masteryIndex = curDeckBuffer.IndexOf(mastery);
				curDeckBuffer.RemoveAt(masteryIndex);
			}

			ChangeState(MasteryUIState.Wait);
		}

		public void ClearMasteryEffect()
		{
			while (selectMasteryDataBuffer.RuntimeItems.Count > 0)
				selectMasteryDataBuffer.RemoveItem(selectMasteryDataBuffer.RuntimeItems[^1]);
		}

		private void ChangeState(MasteryUIState state)
		{
			curState = state;

			switch (curState)
			{
				case MasteryUIState.Wait:
					TimeManager.Instance.Resume();
					break;
				case MasteryUIState.SelectDeck:
					TimeManager.Instance.Pause();
					break;
				case MasteryUIState.SelectCard:
					TimeManager.Instance.Pause();
					break;
				default:
					break;
			}

			selectDeckPanel.SetActive(curState == MasteryUIState.SelectDeck);
			deckPanel.SetActive(curState == MasteryUIState.SelectCard);
			foreach (UIDeck deckUI in deckUIDic.Values)
				deckUI.gameObject.SetActive(false);
		}
	}
}