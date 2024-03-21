using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum QuestType
	{
		Normal,
		NeedWork,
		Achievement
	}

	public enum QuestState
	{
		Locked,
		Unlocked,
		NeedWorkToComplete,
		Working,
		Completed,
		ReceivedReward
	}

	[CreateAssetMenu(fileName = nameof(Quest), menuName = "Variable/" + nameof(Quest))]
	public class Quest : Artifact
	{
		[field: Header("_" + nameof(Quest))]
		[field: SerializeField] public QuestType Type { get; private set; }
		[field: SerializeField] public GameEvent[] GameEvents { get; private set; }
		[field: SerializeField] public Criteria[] Criterias { get; private set; }
		
		[field: NonSerialized] public QuestState State { get; private set; }

		public void Unlock()
		{
			State = QuestState.Unlocked;

			foreach (GameEvent gameEvent in GameEvents)
				gameEvent.AddCallback(TryComplete);
			TryComplete();
		}

		public void TryComplete()
		{
			foreach (Criteria criteria in Criterias)
				if (criteria.IsSatisfied() == false)
					return;

			switch (Type)
			{
				case QuestType.Normal:
					State = QuestState.Completed;
					break;
				case QuestType.NeedWork:
					State = QuestState.NeedWorkToComplete;
					break;
				case QuestType.Achievement:
					State = QuestState.Completed;
					UIManager.Instance.Popup(this);
					break;
			}

			foreach (GameEvent gameEvent in GameEvents)
				gameEvent.RemoveCallback(TryComplete);
		}

		public void StartWork()
		{
			State = QuestState.Working;
		}

		public void ActualComplete()
		{
			State = QuestState.Completed;
		}

		public float GetProgress()
		{
			if (Criterias.Length == 0)
				return 1;

			float progress = 0;
			foreach (Criteria criteria in Criterias)
				progress += criteria.GetProgress();
			return progress /= Criterias.Length;
		}

		public void GetReward()
		{
			if (State == QuestState.Completed)
				State = QuestState.ReceivedReward;

			Debug.Log("GetReward");
		}

		public void Load(QuestData questData)
		{
			State = questData.State;
		}

		public QuestData Save()
		{
			return new QuestData(ID, State);
		}
	}
}