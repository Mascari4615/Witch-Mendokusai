using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class QuestManager
	{
		public QuestBuffer Quests => SOManager.Instance.QuestBuffer;

		public void Init(List<RuntimeQuest> quests)
		{
			Quests.Clear();

			foreach (RuntimeQuest quest in quests)
			{
				Quests.Add(quest);
				quest.StartQuest();
			}
		}

		public void AddQuest(RuntimeQuest quest)
		{
			Quests.Add(quest);
		}

		public RuntimeQuest GetQuest(QuestSO questData)
		{
			return Quests.Datas.Find(x => x.SO?.ID == questData.ID);
		}

		public RuntimeQuest GetQuest(Guid? guid)
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

		public void RemoveQuest(RuntimeQuest quest)
		{
			if (Quests.Remove(quest) == false)
			{
				Debug.Log("Quest not found");
			}
		}

		public void RemoveQuests(QuestType questType)
		{
			Quests.Datas.RemoveAll(x => x.Type == questType);
		}

		public int GetQuestCount(QuestType questType)
		{
			return Quests.Datas.FindAll(x => x.Type == questType).Count;
		}
	}
}