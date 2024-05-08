using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "ST_", menuName = "Variable/" + nameof(StatData))]
	public class StatData : DataSO
	{
		public StatType Type;
	}
}