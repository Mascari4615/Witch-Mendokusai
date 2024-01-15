using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Enemy), menuName = "Variable/Enemy")]
	public class Enemy : Unit
	{
		public ArtifactWithPercentage[] Loots => loots;

		[SerializeField] private ArtifactWithPercentage[] loots;
	}
}