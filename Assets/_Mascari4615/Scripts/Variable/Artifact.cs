using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class Artifact : ScriptableObject
	{
		public const int NONE_ID = -1;

		[field: Header("_" + nameof(Artifact))]
		[field: SerializeField] public int ID { get; set; }
		[field: SerializeField] public string Name { get; set; }
		[field: SerializeField, TextArea] public string Description { get; set; }
		[field: SerializeField] public Sprite Sprite { get; set; }
	}

	[System.Serializable]
	public struct ArtifactWithPercentage
	{
		[field: Header("_" + nameof(ArtifactWithPercentage))]
		[field: SerializeField] public Artifact Artifact { get; set; }
		[field: SerializeField] public float Percentage { get; set; }
	}
}