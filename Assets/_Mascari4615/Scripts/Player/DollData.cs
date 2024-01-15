using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(DollData), menuName = "Variable/Doll")]
	public class DollData : Unit
	{
		public Mastery[] Masteries => masteries;
		[SerializeField] private Mastery[] masteries;
	}
}