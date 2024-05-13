using System;
using System.Collections.Generic;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class Reward
	{
		public static void GetReward(RewardInfoData reward)
		{
			switch (reward.Type)
			{
				case RewardType.Item:
					ItemData itemData = GetItemData(reward.DataSOID);
					SOManager.Instance.ItemInventory.Add(itemData, reward.Amount);
					break;
				case RewardType.Gold:
					SOManager.Instance.Nyang.RuntimeValue += reward.Amount;
					break;
				case RewardType.Exp:
					SOManager.Instance.VQExp.RuntimeValue += reward.Amount;
					break;
			}
		}
	}
}