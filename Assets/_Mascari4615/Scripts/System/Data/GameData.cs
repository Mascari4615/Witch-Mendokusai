using System;
using System.Collections.Generic;
using System.Linq;

namespace Mascari4615
{
	[Serializable]
	public class GameData
	{
		public int curDollIndex = 0;
		public int dummyDollCount = 1;
		public int nyang = 100;

		public List<InventorySlotSaveData> inventoryItems = new();
		public List<DollSaveData> dolls = new();
		public Dictionary<WorkListType, List<Work>> works = new()
		{
			{ WorkListType.DollWork, new() },
			{ WorkListType.DummyWork, new() },
			{ WorkListType.VQuestWork, new() }
		};
		public List<QuestSaveData> quests = new();
		public List<RuntimeQuestSaveData> runtimeQuests = new();
		public Dictionary<StatisticsType, int> statistics = new();
	}
}