using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Skill : Artifact
	{
		[field: SerializeField] public bool AutoUse { get; set; }
		[field: SerializeField] public float Cooltime { get; set; }
		[field: SerializeField] public float PrevDelay { get; set; } = 0;
		[field: SerializeField] public float AfterDelay { get; set; } = 0;

		public void Use(UnitObject unitObject)
		{
			unitObject.StartCoroutine(SkillCoroutine(unitObject));
		}

		public IEnumerator SkillCoroutine(UnitObject unitObject)
		{
			yield return null;
			
			if (PrevDelay > 0)
			{
				SOManager.Instance.IsCooling.RuntimeValue = true;
				yield return new WaitForSeconds(PrevDelay);
				SOManager.Instance.IsCooling.RuntimeValue = false;
			}

			ActualUse(unitObject);

			if (AfterDelay > 0)
			{
				SOManager.Instance.IsCooling.RuntimeValue = true;
				yield return new WaitForSeconds(AfterDelay);
				SOManager.Instance.IsCooling.RuntimeValue = false;
			}
		}

		public abstract void ActualUse(UnitObject unitObject);
	}
}