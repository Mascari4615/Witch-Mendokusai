using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class Artifact : ScriptableObject
	{
		[field: Header("_" + nameof(Artifact))]
		[field: SerializeField] public int ID { get; set; }
		[field: SerializeField] public string Name { get; set; }
		[field: SerializeField, TextArea] public string Description { get; set; }
		[field: SerializeField] public Sprite Thumbnail { get; set; }
	}

	[System.Serializable]
	public struct ArtifactWithPercentage
	{
		[field: Header("_" + nameof(ArtifactWithPercentage))]
		[field: SerializeField] public Artifact Artifact { get; set; }
		[field: SerializeField] public float Percentage { get; set; }
	}
}