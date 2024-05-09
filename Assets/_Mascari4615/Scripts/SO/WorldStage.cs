using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "WS_", menuName = "Data/" + nameof(WorldStage))]
	public class WorldStage : Stage
	{
		[field: SerializeField] public int Temp { get; private set; }
	}
}