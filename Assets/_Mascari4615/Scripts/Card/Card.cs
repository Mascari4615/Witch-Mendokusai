using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Card), menuName = "Variable/" + nameof(Card))]
	public class Card : Artifact
	{
		[field: Header("_" + nameof(Card))]
		[field: SerializeField] public Effect[] Effects { get; private set; }
		[field: SerializeField] public int MaxStack { get; private set; }

		public void OnEquip()
		{
			foreach (Effect effect in Effects)
				effect.OnEquip();
		}

		public void OnRemove()
		{
			foreach (Effect effect in Effects)
				effect.OnRemove();
		}
	}
}