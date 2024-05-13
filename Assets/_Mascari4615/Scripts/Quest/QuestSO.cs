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
			// Debug.Log("Complete Quest: " + name);
			State = QuestState.Completed;
		}

		public QuestSOSaveData Save()
		{
			// Debug.Log($"Save Quest : {name} - {State}");
			QuestSOSaveData questSaveData = new(ID, State);
			return questSaveData;
		}

		public void Load(QuestSOSaveData questSaveData)
		{
			// Debug.Log($"Load Quest: {name} - {questSaveData.State}");
			State = questSaveData.State;
		}
	}
}