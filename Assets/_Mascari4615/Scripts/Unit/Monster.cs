using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Monster), menuName = "Variable/Monster")]
	public class Monster : Unit
	{
		[field: Header("_" + nameof(Monster))]
		[field: SerializeField] public ArtifactWithPercentage[] Loots { get; set; }
	}
}