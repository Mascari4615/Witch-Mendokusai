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

		public void Init(Action<Artifact> cardSelectAction)
		{
			cardSlots = GetComponentsInChildren<UICardSlot>(true);

			for (int i = 0; i < cardSlots.Length; i++)
			{
				cardSlots[i].Init();
				cardSlots[i].SetSelectAction(cardSelectAction);
			}
		}

		public void UpdateUI(List<Mastery> masteries)
		{
			// HashSet : 고유한 값만 저장하는 자료구조
			// Convert masteries to a HashSet for faster lookup
			HashSet<int> masteryIds = new(masteries.Select(m => m.ID));

			foreach (UICardSlot masterySlot in cardSlots)
			{
				bool isTargetMastery = masteryIds.Contains(masterySlot.Artifact.ID);
				masterySlot.SetDisable(!isTargetMastery);
			}
		}
	}
}