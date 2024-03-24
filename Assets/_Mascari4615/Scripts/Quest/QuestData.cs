using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum QuestType
	{
		None = -1,
		Normal,
		VillageRequest,
		Achievement
	}

	public enum QuestDataState
	{
		Locked,
		Unlocked,
		Completed
	}

	[CreateAssetMenu(fileName = nameof(QuestData), menuName = "Variable/" + nameof(QuestData))]
	public class QuestData : Artifact
	{
		[field: Header("_" + nameof(QuestData))]
		[field: SerializeField] public QuestType Type { get; private set; }
		[field: SerializeField] public List<GameEvent> GameEvents { get; private set; }
		[field: SerializeField] public List<Criteria> Criterias { get; private set; }
		[field: SerializeField] public List<Effect> Rewards { get; private set; }

		[field: SerializeField] public float WorkTime { get; private set; }
		[field: SerializeField] public bool AutoComplete { get; private set; }
		[field: SerializeField] public bool AutoWork { get; private set; }
		[field: SerializeField] public bool AutoReward { get; private set; }

		[field: NonSerialized] public QuestDataState State { get; private set; }

		public void Unlock()
		{
			State = QuestDataState.Unlocked;
		}

		public void Complete()
		{
			State = QuestDataState.Completed;
		}

		public QuestSaveData Save()
		{
			return new QuestSaveData(ID, State);
		}

		public void Load(QuestSaveData questSaveData)
		{
			State = questSaveData.State;
		}
	}
}