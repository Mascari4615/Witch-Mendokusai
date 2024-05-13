using System;
using System.Collections.Generic;
using System.Linq;

namespace Mascari4615
{
	[Serializable]
	public struct QuestSOSaveData
	{
		public int QuestID;
		public QuestState State;

		public QuestSOSaveData(int questID, QuestState state)
		{
			QuestID = questID;
			State = state;
		}
	}
}