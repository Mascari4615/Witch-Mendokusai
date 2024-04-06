using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class QuestManager
	{
		public QuestBuffer Quests => SOManager.Instance.QuestBuffer;

		public void Init(List<Quest> quests)
		{
			Quests.Clear();

			foreach (Quest quest in quests)
			{
				Quests.Add(quest);
				quest.StartQuest();
			}
		}

		public void AddQuest(Quest quest)
		{
			Quests.Add(quest);
		}

		public Quest GetQuest(QuestData questData)
		{
			return Quests.Datas.Find(x => x.GetData() == questData);
		}

		public Quest GetQuest(Guid? guid)
		{
			return Quests.Datas.Find(x => x.Guid == guid);
		}

		public void CompleteQuest(Guid? guid)
		{
			GetQuest(guid).Complete();
		}

		public void EndQuestWork(Guid? guid)
		{
			GetQuest(guid).EndWork();
		}

		public void RemoveQuest(Quest quest)
		{
			if (Quests.Remove(quest) == false)
			{
				Debug.Log("Quest not found");
			}
		}
		
		public int GetQuestCount(QuestType questType)
		{
			return Quests.Datas.FindAll(x => x.GetData().Type == questType).Count;
		}
	}
}