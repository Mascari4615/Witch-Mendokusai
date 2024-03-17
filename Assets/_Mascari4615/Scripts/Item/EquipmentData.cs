using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(EquipmentData), menuName = "Variable/EquipmentData")]
	public class EquipmentData : ItemData
	{
		public Card[] Masteries => masteries;
		public Effect[] Effects => effects;
		[SerializeField] private Card[] masteries;
		[SerializeField] private Effect[] effects;
	}
}