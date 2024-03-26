using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class ExpObject : LootObject
	{
		[SerializeField] private int amount;

		private Stat PlayerStat => PlayerController.Instance.PlayerObject.Stat;

		public override void Effect()
		{
			RuntimeManager.PlayOneShot("event:/SFX/EXP", transform.position);
			PlayerStat[StatType.EXP_CUR] += amount;
		}
	}
}