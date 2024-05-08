using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentMagnet : MonoBehaviour
	{
		private Stat PlayerStat => Player.Instance.Stat;

		private void Start()
		{
			PlayerStat.AddListener(StatType.EXP_COLLIDER_SCALE, UpdateEquipment);
			UpdateEquipment();
		}

		public void UpdateEquipment()
		{
			Player.Instance.ExpCollider.transform.localScale =
				Vector3.one * (1 + (PlayerStat[StatType.EXP_COLLIDER_SCALE] * .5f));
		}
	}
}