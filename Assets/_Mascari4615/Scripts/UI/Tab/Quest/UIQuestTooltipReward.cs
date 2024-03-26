using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestTooltipReward : MonoBehaviour
	{
		private UISlot[] slots;

		public void Init()
		{
			slots = GetComponentsInChildren<UISlot>(true);

			foreach (UISlot slot in slots)
			{
				slot.Init();
				slot.gameObject.SetActive(false);
			}
		}

		public void SetReward(Quest quest)
		{
			if (quest == null || quest.Rewards == null)
			{
				foreach (UISlot slot in slots)
					slot.gameObject.SetActive(false);
				return;
			}

			List<RewardData> rewards = quest.Rewards;
			for (int i = 0; i < slots.Length; i++)
			{
				if (i < rewards.Count)
				{
					slots[i].gameObject.SetActive(true);

					switch (rewards[i].Type)
					{
						case RewardType.Item:
							ItemData itemData = DataManager.Instance.ItemDic[rewards[i].ArtifactID];
							slots[i].SetSlot(itemData);
							break;
						case RewardType.Gold:
							slots[i].SetSlot(SOManager.Instance.Nyang, rewards[i].Amount);
							break;
						case RewardType.Exp:
							slots[i].SetSlot(SOManager.Instance.VQExp, rewards[i].Amount);
							break;
					}
				}
				else
				{
					slots[i].gameObject.SetActive(false);
				}
			}
		}
	}
}