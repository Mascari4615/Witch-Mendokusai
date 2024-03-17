using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Dash), menuName = "Skill/" + nameof(Dash))]
	public class Dash : Skill
	{
		public override void ActualUse(UnitObject unitObject)
		{
			unitObject.StartCoroutine(DashLoop());
		}

		private IEnumerator DashLoop()
		{
			SOManager.Instance.IsDashing.RuntimeValue = true;
			yield return new WaitForSeconds(SOManager.Instance.DashDuration.RuntimeValue);
			SOManager.Instance.IsDashing.RuntimeValue = false;
		}
	}
}