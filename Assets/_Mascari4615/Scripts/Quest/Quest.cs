using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "Q_", menuName = "Variable/" + nameof(Quest))]
	public class Quest : DataSO, ISavable<QuestSaveData>
	{
		[field: Header("_" + nameof(Quest))]
		[PropertyOrder(0)][field: SerializeField] public QuestType Type { get; private set; }
		[PropertyOrder(1)][field: SerializeField] public List<GameEventType> GameEvents { get; private set; }
		[PropertyOrder(2)][field: SerializeField] public List<CriteriaInfo> Criterias { get; private set; }
		[PropertyOrder(3)][field: SerializeField] public List<EffectInfo> CompleteEffects { get; private set; }
		[PropertyOrder(4)][field: SerializeField] public List<EffectInfo> RewardEffects { get; private set; }
		[PropertyOrder(5)][field: SerializeField] public List<RewardInfo> Rewards { get; private set; }

		[PropertyOrder(6)][field: SerializeField] public float WorkTime { get; private set; }
		[PropertyOrder(7)][field: SerializeField] public bool AutoWork { get; private set; }
		[PropertyOrder(8)][field: SerializeField] public bool AutoComplete { get; private set; }

		[field: NonSerialized] public QuestState State { get; private set; }

		public void Unlock()
		{
			State = QuestState.Unlocked;
		}

		public void Complete()
		{
			State = QuestState.Completed;
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