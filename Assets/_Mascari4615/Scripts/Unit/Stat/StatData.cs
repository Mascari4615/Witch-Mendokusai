using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "ST_", menuName = "Variable/" + nameof(StatData))]
	public class StatData : DataSO
	{
		[field: SerializeField] public StatType Type { get; set; }
	}
}