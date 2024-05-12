using System;
using System.Collections.Generic;
using System.Linq;

namespace Mascari4615
{
	[Serializable]
	public struct QuestSaveData
	{
		public int QuestID;
		public QuestState State;

		public QuestSaveData(int questID, QuestState state)
		{
			QuestID = questID;
			State = state;
		}
	}
}