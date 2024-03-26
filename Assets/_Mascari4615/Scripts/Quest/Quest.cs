using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Mascari4615
{
	public enum QuestState
	{
		InProgress,
		NeedWorkToComplete,
		Working,
		Completed,
	}

	public class RuntimeCriteria : ICriteria
	{
		public Criteria Criteria { get; private set; }
		// 한 번만 달성하면 되는지
		public bool JustOnce { get; private set; }
		public bool IsCompleted { get; private set; }

		public bool Evaluate()
		{
			if (JustOnce && IsCompleted)
				return true;

			Debug.Log(Criteria.Evaluate());
			return IsCompleted = Criteria.Evaluate();
		}

		public float GetProgress()
		{
			return Criteria.GetProgress();
		}

		// 동적인 조건 정보 (i.e. 에디터 타임에 정해지지 않은, 어떤 아이템이 필요하다)
		[JsonConstructor]
		public RuntimeCriteria(Criteria criteria, bool justOnce = false)
		{
			Criteria = criteria;
			JustOnce = justOnce;
		}

		public RuntimeCriteria(CriteriaInfo criteriaInfo)
		{
			Criteria = criteriaInfo.CriteriaSO.Data;
			JustOnce = criteriaInfo.JustOnce;
		}
	}

	public class Quest
	{
		public Guid? Guid { get; private set; }
		public int DataID { get; private set; }
		public QuestState State { get; private set; }
		public List<RuntimeCriteria> Criterias { get; private set; }

		public QuestData GetData()
		{
			return DataManager.Instance.QuestDic[DataID];
		}
		private QuestData Data => GetData();

		[JsonConstructor]
		public Quest(Guid? guid, int dataID, QuestState state, List<RuntimeCriteria> criterias)
		{
			Guid = guid;
			DataID = dataID;
			State = state;
			Criterias = criterias;

			StartQuest();
		}

		public Quest(QuestData questData)
		{
			Guid = System.Guid.NewGuid();
			DataID = questData.ID;
			Criterias = Data.Criterias.ConvertAll(criteriaData => new RuntimeCriteria(criteriaData));

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
			foreach (RuntimeCriteria criteria in Criterias)
			{
				criteria.Evaluate();
				if (criteria.IsCompleted == false)
				{
					State = QuestState.InProgress;
					return;
				}
			}

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
			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.RemoveCallback(Evaluate);

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
			DataManager.Instance.QuestManager.RemoveQuest(this);
			Data.Complete();

			foreach (GameEvent gameEvent in Data.GameEvents)
				gameEvent.RemoveCallback(Evaluate);

			foreach (Effect completeEffect in Data.CompleteEffects)
				completeEffect.Apply();

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
			foreach (RuntimeCriteria runtimeCriteria in Criterias)
				progress += runtimeCriteria.GetProgress();
			return progress /= Data.Criterias.Count;
		}
	}
}