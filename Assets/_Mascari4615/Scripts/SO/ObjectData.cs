using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "O_", menuName = "Data/" + nameof(ObjectData))]
	public class ObjectData : DataSO
	{
		[field: SerializeField] public GameObject GameObject { get; private set; }
	}
}