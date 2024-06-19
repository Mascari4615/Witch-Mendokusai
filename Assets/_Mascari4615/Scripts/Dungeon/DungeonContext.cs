using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class DungeonContext
	{
		public List<DungeonConstraint> Constraints { get; private set; }

		public DungeonContext(List<DungeonConstraint> constraints)
		{
			Constraints = constraints;
		}

		public UnitStat UpdateMonsterStatByDiff(Unit unit)
		{
			UnitStat newStat = new();

			foreach (DungeonConstraint constraint in Constraints)
			{
				foreach (DungeonConstraintEffectInfo effect in constraint.Effects)
				{
					if (effect.Affiliation != unit.Affiliation)
						continue;

					Debug.Log($"{unit.Name} {effect.StatType} {effect.Value}");
					newStat[effect.StatType] += effect.Value;
				}
			}

			return newStat;
		}
	}
}