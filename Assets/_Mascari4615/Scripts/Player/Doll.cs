using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Doll), menuName = "Variable/Doll")]
	public class Doll : Unit
	{
		[field: SerializeField] public EquipmentData[] EquipmentDatas { get; private set; }
	}
}