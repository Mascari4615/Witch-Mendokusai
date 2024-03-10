using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentMagnet : MonoBehaviour
	{
		public void UpdateEquipment()
		{
			PlayerController.Instance.playerExpCollider.transform.localScale =
				Vector3.one * (1 + (SOManager.Instance.StatDictionary.GetStat("PLAYER_EXP_COLLIDER_SCALE") * .5f));
		}
	}
}