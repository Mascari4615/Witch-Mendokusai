using System;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	[Serializable]
	public struct RewardInfo
	{
		public RewardType Type;
		public DataSO DataSO;
		public int Amount;
	}
}