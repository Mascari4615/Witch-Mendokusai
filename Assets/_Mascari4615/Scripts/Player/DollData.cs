using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(DollData), menuName = "Variable/Doll")]
	public class DollData : Unit
	{
		[field: SerializeField] public EquipmentData[] EquipmentDatas { get; private set; }
	}
}