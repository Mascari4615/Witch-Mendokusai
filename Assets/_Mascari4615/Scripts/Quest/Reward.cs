using System;
using System.Collections.Generic;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public enum RewardType
	{
		Item,
		Gold,
		Exp,
	}

	[Serializable]
	public struct RewardInfo
	{
		public RewardType Type;
		public DataSO DataSO;
		public int Amount;
	}

	[Serializable]
	public struct RewardData
	{
		public RewardType Type;
		public int DataSOID;
		public int Amount;

		public RewardData(RewardInfo rewardInfo)
		{
			Type = rewardInfo.Type;
			DataSOID = rewardInfo.DataSO ? rewardInfo.DataSO.ID : DataSO.NONE_ID;
			Amount = rewardInfo.Amount;
		}
	}

	public class Reward
	{
		public static List<RewardData> InfoToData(List<RewardInfo> rewards) =>
			rewards.ConvertAll((RewardInfo i) => new RewardData(i));

		public static void GetReward(RewardData reward)
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