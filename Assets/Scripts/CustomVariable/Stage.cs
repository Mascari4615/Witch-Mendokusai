using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Stage), menuName = "Variable/Stage")]
	public class Stage : Artifact
	{
		public StageObject Prefab => prefab;
		[SerializeField] private StageObject prefab;
	}
}