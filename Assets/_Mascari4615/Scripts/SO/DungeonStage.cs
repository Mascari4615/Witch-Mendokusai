using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "DS_", menuName = "Data/" + nameof(DungeonStage))]
	public class DungeonStage : Stage
	{
		[field: SerializeField] public int Temp { get; private set; }
	}
}