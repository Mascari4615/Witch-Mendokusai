using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Quest), menuName = "Variable/Quest")]
	public class Quest : Artifact
	{
		public bool Unlock => unlock;
		public bool Complete => complete;
		public GameEvent[] GameEvents => gameEvents;
		public Criteria[] Criterias => criterias;

		[System.NonSerialized] private bool unlock;
		[System.NonSerialized] private bool complete;
		[SerializeField] private GameEvent[] gameEvents;
		[SerializeField] private Criteria[] criterias;

		public void SetComplete()
		{
			complete = true;
		}
	}
}