using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class WorkManager
	{
		public Dictionary<int, List<Work>> Works { get; private set; } = new();

		public void Init(Dictionary<int, List<Work>> works)
		{
			Works = works;
			TimeManager.Instance.AddCallback(TickWorks);
		}

		public void TickWorks()
		{
			foreach (KeyValuePair<int, List<Work>> work in Works)
			{
				for (int i = work.Value.Count - 1; i >= 0; i--)
				{
					work.Value[i].Tick(TimeManager.Tick);
					if (work.Value[i].IsCompleted)
					{
						switch (work.Value[i].workType)
						{
							case WorkType.CompleteQuest:
								// SOManager.Instance.QuestDataBuffer.RuntimeItems[work.Value[i].value].Complete();
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						work.Value.RemoveAt(i);
					}
				}
			}
		}

		public bool DoseDollHaveWork(int dollID)
		{
			return Works.ContainsKey(dollID) && Works[dollID].Count > 0;
		}

		public bool AddWork(int dollID, Work work)
		{
			if (dollID != Doll.DUMMY_ID && DoseDollHaveWork(dollID))
				return false;

			if (Works.ContainsKey(dollID) == false)
				Works.Add(dollID, new List<Work>() { work });
			else
				Works[dollID].Add(work);
			
			return true;
		}

		public void CancleWork(int dollID, Work work)
		{
			if (DoseDollHaveWork(dollID))
				Works[dollID].Remove(work);
		}
	}
}