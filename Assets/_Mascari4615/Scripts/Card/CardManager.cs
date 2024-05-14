using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class CardManager : MonoBehaviour
	{
		private enum CardUIState
		{
			Wait,
			SelectDeck,
			SelectCard,
		}

		// UI
		[SerializeField] private GameObject selectDeckPanel;
		[SerializeField] private List<UISlot> deckSelectButtons;

		[SerializeField] private GameObject deckPanel;
		private readonly Dictionary<int, UIDeck> deckUIDic = new();
		[SerializeField] private List<UICardSlot> cardSelectButtons;

		// Data
		private CardUIState curState = CardUIState.Wait;
		private readonly List<List<CardData>> cardDataBuffers = new(4) { new(), new(), new(), new() };
		private List<int> deckIdMapping = new() { 0, 1, 2, 3 };
		private int curDeckIndex;

		private void Awake()
		{
			UIDeck[] deckUIs = FindObjectsOfType<UIDeck>(true);
			foreach (UIDeck deckUI in deckUIs)
			{
				deckUIDic.Add(deckUI.EquipmentData.ID, deckUI);
				deckUI.Init(cardSelectAction: (slot) => { SelectCard(slot.DataSO as CardData); });
			}

			for (int i = 0; i < deckSelectButtons.Count; i++)
			{
				deckSelectButtons[i].SetSlotIndex(i);
				deckSelectButtons[i].SetClickAction((slot) => { SelectDeck(slot.Index); });
			}

			for (int i = 0; i < cardSelectButtons.Count; i++)
			{
				// cardSelectAction: (slot) => { SelectCard(cardSlots[i].Atrifact as Card); }
				// 원래 위 코드를 썼는데, 클로저 문제로 인해 아래처럼 바꿈
				cardSelectButtons[i].SetClickAction((slot) => { SelectCard(slot.DataSO as CardData); });
			}

			SetState(CardUIState.Wait);
		}

		private void SetState(CardUIState state)
		{
			curState = state;

			switch (curState)
			{
				case CardUIState.Wait:
					TimeManager.Instance.Resume();
					break;
				case CardUIState.SelectDeck:
					TimeManager.Instance.Pause();
					break;
				case CardUIState.SelectCard:
					TimeManager.Instance.Pause();
					break;
				default:
					break;
			}

			selectDeckPanel.SetActive(curState == CardUIState.SelectDeck);
			deckPanel.SetActive(curState == CardUIState.SelectCard);
			foreach (UIDeck deckUI in deckUIDic.Values)
				deckUI.gameObject.SetActive(false);
		}

		public void Init()
		{
			SetState(CardUIState.Wait);
			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;

			while (selectedCardBuffer.Datas.Count > 0)
				selectedCardBuffer.Remove(selectedCardBuffer.Datas[^1]);
			selectedCardBuffer.Clear();

			foreach (List<CardData> cardDataBuffer in cardDataBuffers)
				cardDataBuffer.Clear();
			
			List<EquipmentData> equipments = DataManager.Instance.GetEquipmentDatas(DataManager.Instance.CurDollID);
			for (int i = 0; i < equipments.Count; i++)
				cardDataBuffers[i].AddRange(equipments[i].EffectCards);
		}

		private void ShuffleDeck()
		{
			List<EquipmentData> equipments = DataManager.Instance.GetEquipmentDatas(DataManager.Instance.CurDollID);
			deckIdMapping = deckIdMapping.OrderBy(m => Random.Range(0, 100)).ToList();
			for (int i = 0; i < deckSelectButtons.Count; i++)
				deckSelectButtons[i].SetSlot(equipments[deckIdMapping[i]]);
		}

		public void LevelUp() => StartCoroutine(LevelUp_());
		private IEnumerator LevelUp_()
		{
			TimeManager.Instance.Pause();
			yield return new WaitForSecondsRealtime(1f);
			ShuffleDeck();
			SetState(CardUIState.SelectDeck);
		}

		public void SelectDeck(int selectIndex)
		{
			curDeckIndex = deckIdMapping[selectIndex];
			
			// 선택한 덱에서 카드 뽑기
			List<CardData> curDeckBuffer = cardDataBuffers[curDeckIndex];

			if (curDeckBuffer.Count < 3)
			{
				Debug.LogError("Not Enough Card Count");
				return;
			}

			List<CardData> randomCards = new();
			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;
			while (randomCards.Count < 3)
			{
				int randomIndex = Random.Range(0, curDeckBuffer.Count);
				CardData randomCard = curDeckBuffer[randomIndex];

				if (randomCards.Contains(randomCard))
					continue;

				if (randomCard.MaxStack == 0)
				{
					Debug.LogError("MaxStack is 0");
					continue;
				}

				if (selectedCardBuffer.Datas.Count > 0 &&
					selectedCardBuffer.Datas.Select(m => m.ID == randomCard.ID).Count() == randomCard.MaxStack)
					continue;

				cardSelectButtons[randomCards.Count].SetSlot(randomCard);
				randomCards.Add(randomCard);
			}

			SetState(CardUIState.SelectCard);

			List<EquipmentData> equipmentDatas = DataManager.Instance.GetEquipmentDatas(DataManager.Instance.CurDollID);
			int equipmentID = equipmentDatas[curDeckIndex].ID;
			deckUIDic[equipmentID].SetCards(randomCards);
			deckUIDic[equipmentID].UpdateUI();
			deckUIDic[equipmentID].gameObject.SetActive(true);
		}

		public void SelectCard(CardData card)
		{
			RuntimeManager.PlayOneShot("event:/SFX/UI/Click", transform.position);

			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;

			List<CardData> curDeckBuffer = cardDataBuffers[curDeckIndex];
			selectedCardBuffer.Add(card);

			int sameCardCount = selectedCardBuffer.Datas.Select(m => m == card).Count();
			if (card.MaxStack == sameCardCount)
			{
				int cardIndex = curDeckBuffer.IndexOf(card);
				curDeckBuffer.RemoveAt(cardIndex);
			}

			SetState(CardUIState.Wait);
		}

		public void ClearCardEffect()
		{
			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;
			while (selectedCardBuffer.Datas.Count > 0)
				selectedCardBuffer.Remove(selectedCardBuffer.Datas[^1]);
		}
	}
}