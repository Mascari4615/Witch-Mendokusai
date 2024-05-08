using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(EquipmentData), menuName = "Variable/" + nameof(EquipmentData))]
	public class EquipmentData : ItemData
	{
		[field: SerializeField] public List<CardData> EffectCards { get; private set; }
		[field: SerializeField] public List<EffectInfo> Effects { get; private set; }
		[field: SerializeField] public GameObject Object { get; private set; }
	}
}