using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentMagnet : MonoBehaviour
	{
		public void UpdateEquipment()
		{
			PlayerController.Instance.ExpCollider.transform.localScale =
				Vector3.one * (1 + (SOManager.Instance.StatDictionary.GetStat(Stat.PLAYER_EXP_COLLIDER_SCALE) * .5f));
		}
	}
}