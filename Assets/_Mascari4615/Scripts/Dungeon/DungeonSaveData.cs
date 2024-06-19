using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[Serializable]
	public struct DungeonSaveData
	{
		public Dictionary<int, bool> ConstraintSelected;

		public DungeonSaveData(Dictionary<int, bool> constraintSelected)
		{
			ConstraintSelected = constraintSelected;
		}
	}
}