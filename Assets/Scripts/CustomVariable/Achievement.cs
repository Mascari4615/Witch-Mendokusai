using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Achievement), menuName = "Variable/Achievement")]
	public class Achievement : Artifact
	{
		public bool Unlock => unlock;
		public Criteria Criteria => criteria;

		[SerializeField] private bool unlock;
		[SerializeField] private Criteria criteria;
	}
}