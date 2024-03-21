using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class WorkManager
	{
		public List<Work> DollWorks { get; private set; } = new();
		public List<Work> DummyWorks { get; private set; } = new();

		public void Init(List<Work> dollWorks, List<Work> dummyWorks)
		{
			DollWorks = dollWorks;
			DummyWorks = dummyWorks;
		}

		public void TickEachWorks()
		{
			TickWorks(DollWorks);
			TickWorks(DummyWorks);
		}

		public void TickWorks(List<Work> works)
		{
			for (int i = works.Count - 1; i >= 0 ; i--)
			{
				Work work = works[i];
				work.Tick();
				if (work.IsCompleted())
				{
					switch (work.WorkType)
					{
						case WorkType.CompleteQuest:
							DataManager.Instance.QuestManager.CompleteQuest(work.Value);
							break;
					}
					works.RemoveAt(i);
				}
			}
		}

		public bool TryGetWorkByDollID(int dollID, out Work targetWork)
		{
			foreach (Work work in DollWorks)
			{
				if (work.DollID == dollID)
				{
					targetWork = work;
					return true;
				}
			}
			targetWork = null;
			return false;
		}

		public bool TryGetWorkByQuestGuid(Guid? questGuid, out Work targetWork)
		{
			foreach (Work work in DollWorks)
			{
				if (work.WorkType == WorkType.CompleteQuest && work.Value == questGuid)
				{
					targetWork = work;
					return true;
				}
			}
			targetWork = null;
			return false;
		}

		public void AddWork(Work work)
		{
			if (work.DollID == Doll.DUMMY_ID)
			{
				DummyWorks.Add(work);
			}
			else if (TryGetWorkByDollID(work.DollID, out _) == false)
			{
				DollWorks.Add(work);
			}
		}

		public void CancleWork(int dollID)
		{
			for (int i = DollWorks.Count - 1; i >= 0; i--)
			{
				if (DollWorks[i].DollID == dollID)
				{
					DollWorks.RemoveAt(i);
					return;
				}
			}
		}
	}
}