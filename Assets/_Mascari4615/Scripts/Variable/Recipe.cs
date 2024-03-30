using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Recipe), menuName = "Variable/Recipe")]
	public class Recipe : ScriptableObject
	{
		[field: SerializeField] public ItemData[] Ingredients { get; private set; }
		[field: SerializeField] public float Percentage { get; private set; }
	}
}