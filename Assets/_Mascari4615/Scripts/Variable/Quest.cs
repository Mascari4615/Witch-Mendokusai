using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum QuestType
	{
		Normal,
		VillageQuest
	}

	[CreateAssetMenu(fileName = nameof(Quest), menuName = "Variable/" + nameof(Quest))]
	public class Quest : Artifact
	{
		[field: Header("_" + nameof(Quest))]
		[field: SerializeField] public QuestType QuestType { get; private set; }
		[field: SerializeField] public GameEvent[] GameEvents { get; private set; }
		[field: SerializeField] public Criteria[] Criterias { get; private set; }
		[field: System.NonSerialized] public bool IsUnlocked { get; private set; }
		[field: System.NonSerialized] public bool IsCompleted { get; private set; }

		public void Unlock()
		{
			IsUnlocked = true;
			foreach (GameEvent gameEvent in GameEvents)
				gameEvent.AddCallback(CheckComplete);
			CheckComplete();
		}

		public void Complete()
		{
			IsCompleted = true;
			foreach (GameEvent gameEvent in GameEvents)
				gameEvent.RemoveCallback(CheckComplete);
		
			UIManager.Instance.Popup(this);
		}

		public void CheckComplete()
		{
			foreach (Criteria criteria in Criterias)
				if (criteria.HasComplete() == false)
					return;

			Complete();
		}
	}
}