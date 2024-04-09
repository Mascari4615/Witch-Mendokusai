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
	public class CardManager : MonoBehaviour
	{
		private enum CardUIState
		{
			Wait,
			SelectDeck,
			SelectCard,
		}

		private readonly List<List<Card>> cardDataBuffers = new(3) { new(), new(), new() };
		private int curDeckIndex;

		[SerializeField] private GameObject selectDeckPanel;
		[SerializeField] private GameObject deckPanel;
		[SerializeField] private UISlot[] deckSelectButtons;
		[SerializeField] private UICardSlot[] cardSelectButtons;
		private readonly Dictionary<int, UIDeck> deckUIDic = new();
		private CardUIState curState = CardUIState.Wait;

		private void Awake()
		{
			UIDeck[] deckUIs = FindObjectsOfType<UIDeck>(true);
			foreach (UIDeck deckUI in deckUIs)
				deckUIDic.Add(deckUI.EquipmentData.ID, deckUI);

			ChangeState(CardUIState.Wait);
		}

		private void Start()
		{
			SOManager.Instance.OnLevelUp.AddCallback(LevelUp);
		}

		public void Init()
		{
			ChangeState(CardUIState.Wait);
			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;

			while (selectedCardBuffer.Datas.Count > 0)
				selectedCardBuffer.Remove(selectedCardBuffer.Datas[^1]);
			selectedCardBuffer.Clear();

			foreach (List<Card> cardDataBuffer in cardDataBuffers)
				cardDataBuffer.Clear();

			for (int i = 0; i < 3; i++)
			{
				EquipmentData equipment = DataManager.Instance.GetEquipment(DataManager.Instance.CurDollID, i);

				if (equipment == null)
					continue;
				cardDataBuffers[i].AddRange(equipment.Masteries);

				// cardSelectAction: (slot) => { SelectCard(cardSlots[i].Atrifact as Card); }
				// 원래 위 코드를 썼는데, 클로저 문제로 인해 아래처럼 바꿈

				deckUIDic[equipment.ID].Init(cardSelectAction: (slot) => { SelectCard(slot.DataSO as Card); });

				deckSelectButtons[i].SetSlotIndex(i);
				deckSelectButtons[i].SetSlot(equipment);
				deckSelectButtons[i].SetClickAction((slot) => { SelectDeck(slot.Index); });

				cardSelectButtons[i].SetClickAction((slot) => { SelectCard(slot.DataSO as Card); });
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
			ChangeState(CardUIState.SelectDeck);
		}

		public void SelectDeck(int deckIndex)
		{
			curDeckIndex = deckIndex;
			List<Card> curDeckBuffer = cardDataBuffers[deckIndex];

			if (curDeckBuffer.Count < 3)
			{
				Debug.LogError("Not Enough Card Count");
				return;
			}

			List<Card> randomCards = new();
			while (randomCards.Count < 3)
			{
				int randomIndex = Random.Range(0, curDeckBuffer.Count);
				Card randomCard = curDeckBuffer[randomIndex];

				if (randomCards.Contains(randomCard))
					continue;

				cardSelectButtons[randomCards.Count].SetSlot(randomCard);
				randomCards.Add(randomCard);
			}

			ChangeState(CardUIState.SelectCard);

			int equipmentID = DataManager.Instance.GetEquipment(DataManager.Instance.CurDollID, deckIndex).ID;
			deckUIDic[equipmentID].UpdateUI(randomCards);
			deckUIDic[equipmentID].gameObject.SetActive(true);
		}

		public void SelectCard(Card card)
		{
			RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);

			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;

			List<Card> curDeckBuffer = cardDataBuffers[curDeckIndex];
			selectedCardBuffer.Add(card);

			int sameCardCount = selectedCardBuffer.Datas.Select(m => m == card).Count();
			if (card.MaxStack == sameCardCount)
			{
				int cardIndex = curDeckBuffer.IndexOf(card);
				curDeckBuffer.RemoveAt(cardIndex);
			}

			ChangeState(CardUIState.Wait);
		}

		public void ClearCardEffect()
		{
			CardBuffer selectedCardBuffer = SOManager.Instance.SelectedCardBuffer;
			while (selectedCardBuffer.Datas.Count > 0)
				selectedCardBuffer.Remove(selectedCardBuffer.Datas[^1]);
		}

		private void ChangeState(CardUIState state)
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
	}
}