using System;
using System.Collections.Generic;

namespace Mascari4615
{
	// NPC가 다루는 UI 정보
	[Serializable]
	public struct NPCPanelInfo
	{
		public NPCType Type;
		public List<DataSO> DataSOs;
	}
}