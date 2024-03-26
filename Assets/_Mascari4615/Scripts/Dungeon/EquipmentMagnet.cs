using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentMagnet : MonoBehaviour
	{
		private Stat PlayerStat => PlayerController.Instance.PlayerObject.Stat;

		public void UpdateEquipment()
		{
			PlayerController.Instance.ExpCollider.transform.localScale =
				Vector3.one * (1 + (PlayerStat[StatType.PLAYER_EXP_COLLIDER_SCALE] * .5f));
		}
	}
}