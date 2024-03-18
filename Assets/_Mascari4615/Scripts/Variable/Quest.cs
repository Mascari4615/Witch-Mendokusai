using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Quest), menuName = "Variable/" + nameof(Quest))]
	public class Quest : Artifact
	{
		[field: SerializeField] public GameEvent[] GameEvents { get; private set; }
		[field: SerializeField] public Criteria[] Criterias{ get; private set; }
		[System.NonSerialized] public bool IsUnlocked;
		[System.NonSerialized] public bool IsCompleted;

		public void Unlock()
		{
			IsUnlocked = true;
		}

		public void SetComplete()
		{
			IsCompleted = true;
		}
	}
}