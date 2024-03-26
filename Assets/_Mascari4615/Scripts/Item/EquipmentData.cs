using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(EquipmentData), menuName = "Variable/" + nameof(EquipmentData))]
	public class EquipmentData : ItemData
	{
		[field: SerializeField] public List<Card> Masteries { get; private set; }
		[field: SerializeField] public List<Effect> Effects { get; private set; }
	}
}