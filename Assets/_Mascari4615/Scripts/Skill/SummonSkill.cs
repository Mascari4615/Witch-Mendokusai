using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(SummonSkill), menuName = "Skill/SummonSkill")]
	public class SummonSkill : Skill
	{
		[SerializeField] private GameObject prefab;

		public override bool Use(UnitObject unitObject)
		{
			var o = ObjectManager.Instance.PopObject(prefab);

			o.transform.position = unitObject.transform.position;

			if (o.TryGetComponent(out SkillObject skillObject))
			{
				skillObject.InitContext(unitObject);
			}

			o.SetActive(true);

			return true;
		}
	}
}