using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Mastery), menuName = "Variable/Mastery")]
	public class Mastery : Artifact
	{
		[field: Header("_" + nameof(Mastery))]
		[field: SerializeField] public Effect[] Effects { get; private set; }

		public void OnEquip()
		{
			foreach (var effect in Effects)
				effect.OnEquip();
		}

		public void OnRemove()
		{
			foreach (var effect in Effects)
				effect.OnRemove();
		}
	}
}