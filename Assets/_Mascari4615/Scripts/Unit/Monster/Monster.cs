using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Monster), menuName = "Variable/" + nameof(Unit) + "/" + nameof(Monster))]
	public class Monster : Unit
	{
		[field: Header("_" + nameof(Monster))]
		[field: SerializeField] public List<DataSOWithPercentage> Loots { get; private set; }
	}
}