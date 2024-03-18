using System.Linq;

namespace Mascari4615
{
	public class UIQuestBuffer : UIDataBuffer<Quest>
	{
		public override void UpdateUI()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				UIQuestSlot slot = Slots[i] as UIQuestSlot;
				Quest quest = dataBuffer.RuntimeItems.ElementAtOrDefault(i);

				if (quest == null)
				{
					slot.SetArtifact(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					bool slotActive = quest.IsUnlocked;

					slot.SetArtifact(quest);
					slot.gameObject.SetActive(slotActive);
				}
			}
		}
	}
}