using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class RuntimeQuest : ISavable<RuntimeQuestSaveData>
	{
		public Guid? Guid { get; private set; }
		public int DataID { get; private set; }
		public RuntimeQuestState State { get; private set; }
		public List<RuntimeCriteria> Criterias { get; private set; }
		public List<RewardData> Rewards { get; private set; }

		public Quest GetData()
		{
			return GetQuest(DataID);
		}
		private Quest Data => GetData();

		public RuntimeQuest(RuntimeQuestSaveData saveData)
		{
			Load(saveData);
			StartQuest();
		}

		public RuntimeQuest(Quest questData)
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
				Data.GameEvents.Add(GameEventType.OnTick);
			foreach (GameEventType gameEventType in Data.GameEvents)
				GameEventManager.Instance.RegisterCallback(gameEventType, Evaluate);
			Evaluate();
		}

		public void Evaluate()
		{
			if (Data.Type == QuestType.VillageRequest)
			{
				if (State >= RuntimeQuestState.Working)
					return;
			}

			foreach (RuntimeCriteria criteria in Criterias)
			{
				criteria.Evaluate();
				if (criteria.IsCompleted == false)
				{
					State = RuntimeQuestState.InProgress;
					return;
				}
			}

			if (Data.Type == QuestType.VillageRequest)
			{
				State = RuntimeQuestState.CanWork;
				if (Data.AutoWork)
					StartWork();
			}
			else
			{
				State = RuntimeQuestState.CanComplete;
			}
		}

		public void StartWork(int workerID = WorkManager.NONE_WORKER_ID)
		{
			State = RuntimeQuestState.Working;
			
			foreach (GameEventType gameEventType in Data.GameEvents)
				GameEventManager.Instance.UnregisterCallback(gameEventType, Evaluate);
			Work work = new(workerID, WorkType.QuestWork, Guid, Data.WorkTime);
			DataManager.Instance.WorkManager.AddWork(work);
		}

		public void EndWork()
		{
			State = RuntimeQuestState.CanComplete;

			if (Data.AutoComplete)
				Complete();
		}

		public void Complete()
		{
			State = RuntimeQuestState.Completed;
		
			DataManager.Instance.QuestManager.RemoveQuest(this);
			Data.Complete();

			foreach (GameEventType gameEventType in Data.GameEvents)
				GameEventManager.Instance.UnregisterCallback(gameEventType, Evaluate);
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
			if (State == RuntimeQuestState.Working)
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

		public void Load(RuntimeQuestSaveData saveData)
		{
			Guid = saveData.Guid;
			DataID = saveData.DataID;
			State = saveData.State;
			Criterias = saveData.Criterias.ConvertAll(criteriaData => new RuntimeCriteria(criteriaData));
			Rewards = saveData.Rewards;
		}

		public RuntimeQuestSaveData Save()
		{
			return new RuntimeQuestSaveData
			{
				Guid = Guid,
				DataID = DataID,
				State = State,
				Criterias = Criterias.ConvertAll(criteria => criteria.Save()),
				Rewards = Rewards
			};
		}
	}
}