using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentMagnet : MonoBehaviour
	{
		[SerializeField] private StatDictionary statDictionary;

		public void UpdateEquipment()
		{
			PlayerController.Instance.playerExpCollider.transform.localScale = Vector3.one * (2 + statDictionary.GetStat("PLAYER_EXP_COLLIDER_SCALE"));
		}
	}
}