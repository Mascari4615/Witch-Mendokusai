using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "C_", menuName = "Variable/" + nameof(CardData))]
	public class CardData : DataSO
	{
		[field: Header("_" + nameof(CardData))]
		[field: SerializeField] public Effect[] Effects { get; private set; }
		[field: SerializeField] public int MaxStack { get; private set; }

		public void OnEquip()
		{
			foreach (Effect effect in Effects)
				effect.Apply();
		}

		public void OnRemove()
		{
			foreach (Effect effect in Effects)
				effect.Cancle();
		}
	}
}