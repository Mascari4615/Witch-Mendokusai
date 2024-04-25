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

	[CreateAssetMenu(fileName = "Q_", menuName = "Variable/" + nameof(QuestData))]
	public class QuestData : DataSO, ISavable<QuestDataSave>
	{
		[field: Header("_" + nameof(QuestData))]
		[PropertyOrder(0)] [field: SerializeField] public QuestType Type { get; private set; }
		[PropertyOrder(1)] [field: SerializeField] public List<GameEvent> GameEvents { get; private set; }
		[PropertyOrder(2)] [field: SerializeField] public List<CriteriaInfo> Criterias { get; private set; }
		[PropertyOrder(3)] [field: SerializeField] public List<Effect> CompleteEffects { get; private set; }
		[PropertyOrder(4)] [field: SerializeField] public List<Effect> RewardEffects { get; private set; }
		[PropertyOrder(5)] [field: SerializeField] public List<RewardInfo> Rewards { get; private set; }

		[PropertyOrder(6)] [field: SerializeField] public float WorkTime { get; private set; }
		[PropertyOrder(7)] [field: SerializeField] public bool AutoWork { get; private set; }
		[PropertyOrder(8)] [field: SerializeField] public bool AutoComplete { get; private set; }

		[field: NonSerialized] public QuestDataState State { get; private set; }

		public void Unlock()
		{
			State = QuestDataState.Unlocked;
		}

		public void Complete()
		{
			State = QuestDataState.Completed;
		}

		public QuestDataSave Save()
		{
			return new QuestDataSave(ID, State);
		}

		public void Load(QuestDataSave questSaveData)
		{
			State = questSaveData.State;
		}
	}
}