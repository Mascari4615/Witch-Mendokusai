using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "Q_", menuName = "Variable/" + nameof(QuestSO))]
	public class QuestSO : DataSO, ISavable<QuestSOSaveData>
	{
		[field: Header("_" + nameof(QuestSO))]
		[PropertyOrder(100)][field: SerializeField] public QuestInfo Data { get; private set; }

		[field: NonSerialized] public QuestState State { get; private set; }

		public void Unlock()
		{
			State = QuestState.Unlocked;
		}

		public void Complete()
		{
			State = QuestState.Completed;
		}

		public QuestSOSaveData Save()
		{
			return new QuestSOSaveData(ID, State);
		}

		public void Load(QuestSOSaveData questSaveData)
		{
			State = questSaveData.State;
		}
	}
}