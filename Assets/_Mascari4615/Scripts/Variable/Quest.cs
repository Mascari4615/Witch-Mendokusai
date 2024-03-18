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
		[System.NonSerialized] public bool Unlock;
		[System.NonSerialized] public bool Complete;

		public void SetComplete()
		{
			Complete = true;
		}
	}
}