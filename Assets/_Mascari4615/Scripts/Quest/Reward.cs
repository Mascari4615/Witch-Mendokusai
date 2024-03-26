using System;

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
		public Artifact Artifact;
		public int Amount;
	}

	[Serializable]
	public struct RewardData
	{
		public RewardType Type;
		public int ArtifactID;
		public int Amount;

		public RewardData(RewardInfo rewardInfo)
		{
			Type = rewardInfo.Type;
			ArtifactID = rewardInfo.Artifact ? rewardInfo.Artifact.ID : Artifact.NONE_ID;
			Amount = rewardInfo.Amount;
		}
	}

	public class Reward
	{
		public static void GetReward(RewardData reward)
		{
			switch (reward.Type)
			{
				case RewardType.Item:
					ItemData itemData = DataManager.Instance.ItemDic[reward.ArtifactID];
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