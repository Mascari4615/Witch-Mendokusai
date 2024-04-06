using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestDataBuffer : UIDataBuffer<QuestData>
	{

		public override void UpdateUI()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				UIQuestSlot slot = Slots[i] as UIQuestSlot;
				QuestData quest = DataBuffer.RuntimeItems.ElementAtOrDefault(i);

				if (quest == null)
				{
					slot.SetSlot(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					// bool slotActive = quest.State > QuestState.Wait;

					slot.SetSlot(quest);
					// slot.gameObject.SetActive(slotActive);
					slot.gameObject.SetActive(true);
				}
			}
		}
	}
}