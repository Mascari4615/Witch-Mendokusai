using UnityEngine;

namespace Mascari4615
{
	public abstract class Stage : Artifact
	{
		[field: SerializeField] public StageObject Prefab { get; private set; }
	}
}