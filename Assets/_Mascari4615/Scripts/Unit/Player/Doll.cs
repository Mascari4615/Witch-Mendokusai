using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Doll), menuName = "Variable/" + nameof(Unit) +"/"+ nameof(Doll))]
	public class Doll : Unit
	{
		[field: Header("_" + nameof(Doll))]
		[field: SerializeField] public EquipmentData[] EquipmentDatas { get; private set; }
	}
}