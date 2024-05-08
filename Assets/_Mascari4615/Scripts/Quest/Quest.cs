using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public enum QuestState
	{
		InProgress,
		CanWork,
		Working,
		CanComplete,
		Completed,
	}

	public class Quest
	{
		public Guid? Guid { get; private set; }
		public int DataID { get; private set; }
		public QuestState State { get; private set; }
		public List<RuntimeCriteria> Criterias { get; private set; }
		public List<RewardData> Rewards { get; private set; }

		public QuestData GetData()
		{
			return GetQuest(DataID);
		}
		private QuestData Data => GetData();

		[JsonConstructor]
		public Quest(Guid? guid, int dataID, QuestState state, List<RuntimeCriteria> criterias, List<RewardData> rewards)
		{
			Guid = guid;
			DataID = dataID;
			State = state;
			Criterias = criterias;
			Rewards = rewards;

			StartQuest();
		}

		public Quest(QuestData questData)
		{
			Guid = System.Guid.NewGuid();
			DataID = questData.ID;
			Criterias = Data.Criterias.ConvertAll(criteriaData => new RuntimeCriteria(criteriaData));
			Rewards = Data.Rewards.ConvertAll(rewardData => new RewardData(rewardData));

			StartQuest();
		}

		public void StartQuest()
		{
			if (Data.AutoComplete)
				Data.GameEvents.Add(SOManager.Instance.OnTick);
			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.AddCallback(Evaluate);
			Evaluate();
		}

		public void Evaluate()
		{
			if (Data.Type == QuestType.VillageRequest)
			{
				if (State >= QuestState.Working)
					return;
			}

			foreach (RuntimeCriteria criteria in Criterias)
			{
				criteria.Evaluate();
				if (criteria.IsCompleted == false)
				{
					State = QuestState.InProgress;
					return;
				}
			}

			if (Data.Type == QuestType.VillageRequest)
			{
				State = QuestState.CanWork;
				if (Data.AutoWork)
					StartWork();
			}
			else
			{
				State = QuestState.CanComplete;
			}
		}

		public void StartWork(int workerID = WorkManager.NONE_WORKER_ID)
		{
			State = QuestState.Working;
			
			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.RemoveCallback(Evaluate);

			Work work = new(workerID, WorkType.QuestWork, Guid, Data.WorkTime);
			DataManager.Instance.WorkManager.AddWork(work);
		}

		public void EndWork()
		{
			State = QuestState.CanComplete;

			if (Data.AutoComplete)
				Complete();
		}

		public void Complete()
		{
			State = QuestState.Completed;
		
			DataManager.Instance.QuestManager.RemoveQuest(this);
			Data.Complete();

			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.RemoveCallback(Evaluate);

			Effect.ApplyEffects(Data.CompleteEffects);

			if (Data.Type == QuestType.Achievement)
			{
				UIManager.Instance.Popup(Data);
			}

			GetReward();
		}

		private void GetReward()
		{
			Effect.ApplyEffects(Data.RewardEffects);

			foreach (RewardData rewardData in Rewards)
				Reward.GetReward(rewardData);
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

			if (Criterias.Count == 0)
				return 1;

			float progress = 0;
			foreach (RuntimeCriteria runtimeCriteria in Criterias)
				progress += runtimeCriteria.GetProgress();
			return progress /= Criterias.Count;
		}
	}
}