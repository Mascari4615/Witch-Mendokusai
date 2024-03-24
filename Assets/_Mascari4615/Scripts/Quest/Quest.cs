using System;
using UnityEngine;

namespace Mascari4615
{
	public enum QuestState
	{
		Wait,
		NeedWorkToComplete,
		Working,
		Completed,
	}

	public class Quest
	{
		public Guid? Guid { get; private set; } = null;
		public QuestData Data { get; private set; } = null;
		public QuestState State { get; private set; }

		public Quest(Guid? guid, QuestData data)
		{
			Guid = guid;
			Data = data;
			State = QuestState.Wait;

			if (Data.AutoComplete)
				Data.GameEvents.Add(SOManager.Instance.OnTick);
			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.AddCallback(TryComplete);
			TryComplete();
		}

		public void TryComplete()
		{
			foreach (Criteria criteria in Data.Criterias)
				if (criteria.IsSatisfied() == false)
					return;

			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.RemoveCallback(TryComplete);

			switch (Data.Type)
			{
				case QuestType.Normal:
					State = QuestState.Completed;
					break;
				case QuestType.VillageRequest:
					State = QuestState.NeedWorkToComplete;
					if (Data.AutoWork)
						StartWork();
					break;
				case QuestType.Achievement:
					State = QuestState.Completed;
					UIManager.Instance.Popup(Data);
					break;
			}
		}

		public void StartWork(int workerID = WorkManager.NONE_WORKER_ID)
		{
			Work work = new(workerID, WorkType.CompleteQuest, Guid, Data.WorkTime);
			DataManager.Instance.WorkManager.AddWork(work);
			State = QuestState.Working;
		}

		public void WorkEnd()
		{
			State = QuestState.Completed;
			// TODO : Reward 없으면 바로 리스트에서 제거

			if (Data.AutoReward)
				GetReward();
		}

		public void GetReward()
		{
			Debug.Log("GetReward");
			Data.Complete();
			DataManager.Instance.QuestManager.RemoveQuest(this);
			foreach (Effect reward in Data.Rewards)
				reward.Apply();
		}

		public float GetProgress()
		{
			if (State == QuestState.Working)
			{
				if (DataManager.Instance.WorkManager.TryGetWorkByQuestGuid(Guid, out Work work))
				{
					return work.GetProgress();
				}
				return 0;
			}

			if (Data.Criterias.Count == 0)
				return 1;

			float progress = 0;
			foreach (Criteria criteria in Data.Criterias)
				progress += criteria.GetProgress();
			return progress /= Data.Criterias.Count;
		}

		public void Load(QuestSlotData questData)
		{
			State = questData.State;
		}

		public QuestSlotData Save()
		{
			return new QuestSlotData(this);
		}
	}
}