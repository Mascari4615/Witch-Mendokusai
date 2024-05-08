
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "DC_", menuName = "Variable/" + nameof(DungeonConstraint))]
	public class DungeonConstraint : DataSO
	{
		[field:SerializeField] public List<IEffect> Effects { get; private set; } = new();
		[field: NonSerialized] public bool IsSelected { get; private set; } = false;

		public void Toggle()
		{
			IsSelected = !IsSelected;
		}
	}
}