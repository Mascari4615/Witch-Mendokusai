using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mascari4615
{
	public enum QuestType
	{
		Normal,
		VillageQuest
	}

	public enum QuestState
	{
		Locked,
		Unlocked,
		NeedWorkToComplete,
		Completed,
		ReceivedReward
	}

	[CreateAssetMenu(fileName = nameof(Quest), menuName = "Variable/" + nameof(Quest))]
	public class Quest : Artifact
	{
		[field: Header("_" + nameof(Quest))]
		[field: SerializeField] public QuestType QuestType { get; private set; }
		[field: SerializeField] public GameEvent[] GameEvents { get; private set; }
		[field: SerializeField] public Criteria[] Criterias { get; private set; }
		[field: System.NonSerialized] public QuestState QuestState { get; private set; }
		[field: System.NonSerialized] public Work Work { get; private set; }

		public void Unlock()
		{
			QuestState = QuestState.Unlocked;

			foreach (GameEvent gameEvent in GameEvents)
				gameEvent.AddCallback(CheckComplete);
			CheckComplete();
		}

		public void CheckComplete()
		{
			foreach (Criteria criteria in Criterias)
				if (criteria.HasComplete() == false)
					return;

			Complete();
		}

		public void Complete()
		{
			if (QuestType == QuestType.VillageQuest)
			{
				QuestState = QuestState.NeedWorkToComplete;
			}
			else
			{
				QuestState = QuestState.Completed;
			}

			foreach (GameEvent gameEvent in GameEvents)
				gameEvent.RemoveCallback(CheckComplete);
			UIManager.Instance.Popup(this);
		}

		public void ActualComplete()
		{
			QuestState = QuestState.Completed;
		}

		public void SetWork(Work work)
		{
			Work = work;
		}

		public float GetProgress()
		{
			if (QuestState == QuestState.NeedWorkToComplete)
			{
				if (Work != null)
					return Work.Progress;
				else
					return 1;
			}
			else
			{
				if (Criterias.Length == 0)
				{
					return 1;
				}
				else
				{
					float progress = 0;
					foreach (Criteria criteria in Criterias)
						progress += criteria.GetProgress();
					return progress /= Criterias.Length;
				}
			}
		}

		public void GetReward()
		{
			if (QuestState == QuestState.Completed)
				QuestState = QuestState.ReceivedReward;

			Debug.Log("GetReward");
		}
	}
}