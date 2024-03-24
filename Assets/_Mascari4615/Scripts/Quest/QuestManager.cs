using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class QuestManager
	{
		private List<Quest> Quests => SOManager.Instance.QuestBuffer.RuntimeItems;

		public void Init(List<QuestSlotData> questDatas)
		{
			Quests.Clear();

			foreach (QuestSlotData questData in questDatas)
			{
				Quest quest = new(questData.Guid, DataManager.Instance.QuestDic[questData.QuestID]);
				Quests.Add(quest);
			}
		}

		public void AddQuest(Quest quest)
		{
			Quests.Add(quest);
		}

		public Quest GetQuest(QuestData questData)
		{
			return Quests.Find(x => x.Data == questData);
		}

		public Quest GetQuest(Guid? guid)
		{
			return Quests.Find(x => x.Guid == guid);
		}

		public void CompleteQuest(Guid? guid)
		{
			GetQuest(guid).WorkEnd();
		}

		public void RemoveQuest(Quest quest)
		{
			Quests.Remove(quest);
		}
		
		public int GetQuestCount(QuestType questType)
		{
			return Quests.FindAll(x => x.Data.Type == questType).Count;
		}

		public List<QuestSlotData> Save()
		{
			List<QuestSlotData> questDatas = new();

			foreach (Quest quest in Quests)
				questDatas.Add(new QuestSlotData(quest));

			return questDatas;
		}
	}
}