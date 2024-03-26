using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class QuestManager
	{
		public List<Quest> Quests => SOManager.Instance.QuestBuffer.RuntimeItems;

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
			return Quests.Find(x => x.GetData() == questData);
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
			return Quests.FindAll(x => x.GetData().Type == questType).Count;
		}
	}
}