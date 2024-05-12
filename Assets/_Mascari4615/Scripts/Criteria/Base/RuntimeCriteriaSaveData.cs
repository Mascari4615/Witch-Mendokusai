using System;

namespace Mascari4615
{
	[Serializable]
	public struct RuntimeCriteriaSaveData
	{
		public CriteriaInfoSaveData CriteriaInfo;
		public bool JustOnce;
		public bool IsCompleted;
	}
}