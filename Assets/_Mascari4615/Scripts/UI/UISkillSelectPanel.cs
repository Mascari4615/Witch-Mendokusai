using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIMasterySelectPanel : MonoBehaviour
	{
		[SerializeField] private MasteryManager masteryManager;
		[SerializeField] private UIMasterySlot[] masterySlots;

		public void Init()
		{
			for (int i = 0; i < masterySlots.Length; i++)
			{
				masterySlots[i].Init();
				masterySlots[i].SetSelectAction((Artifact a) =>
				{
					// masteryManager.SelectMastery(masterySlots[i].Atrifact as Mastery);
					// 원래 위 코드를 썼는데, 클로저 문제로 인해 아래처럼 바꿈
					
					masteryManager.SelectMastery(a as Mastery);
				});
			}
		}

		public void UpdateUI(List<Mastery> masteries)
		{
			foreach (UIMasterySlot masterySlot in masterySlots)
			{
				bool isTargetMastery = false;

				foreach (Mastery mastery in masteries)
				{
					if (masterySlot.Artifact.ID == mastery.ID)
					{
						isTargetMastery = true;
						break;
					}
				}
				masterySlot.SetDisable(!isTargetMastery);
			}
		}
	}
}