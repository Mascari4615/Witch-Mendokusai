using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIDeck : MonoBehaviour
	{
		[field: SerializeField] public EquipmentData EquipmentData { get; private set; }
		private UICardSlot[] cardSlots;

		public void Init(Action<UISlot> cardSelectAction)
		{
			cardSlots = GetComponentsInChildren<UICardSlot>(true);

			for (int i = 0; i < cardSlots.Length; i++)
			{
				cardSlots[i].Init();
				cardSlots[i].SetClickAction(cardSelectAction);
			}
		}

		public void UpdateUI(List<Card> cards)
		{
			// HashSet : 고유한 값만 저장하는 자료구조
			// Convert masteries to a HashSet for faster lookup
			HashSet<int> cardIds = new(cards.Select(m => m.ID));

			foreach (UICardSlot cardSlot in cardSlots)
			{
				bool isTargetCard = cardIds.Contains(cardSlot.DataSO.ID);
				cardSlot.SetDisable(!isTargetCard);
			}
		}
	}
}