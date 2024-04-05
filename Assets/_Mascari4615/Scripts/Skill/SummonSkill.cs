using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(SummonSkill), menuName = "Skill/SummonSkill")]
	public class SummonSkill : Skill
	{
		[SerializeField] private GameObject prefab;
		[SerializeField] private bool setRotation;

		public override void ActualUse(UnitObject unitObject)
		{
			GameObject o = ObjectManager.Instance.PopObject(prefab);
			o.transform.position = unitObject.transform.position;

			if (setRotation)
			{
				// 공격 위치를 향하도록 회전
				o.transform.rotation = Quaternion.LookRotation(SOManager.Instance.PlayerAimDirection.RuntimeValue);
			}

			if (o.TryGetComponent(out SkillObject skillObject))
				skillObject.InitContext(unitObject);

			o.SetActive(true);
		}
	}
}