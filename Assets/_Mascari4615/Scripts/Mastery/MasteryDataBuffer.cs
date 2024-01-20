using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(MasteryDataBuffer), menuName = "GameSystem/DataBuffer/Mastery")]
	public class MasteryDataBuffer : DataBuffer<Mastery>
	{
		[SerializeField] private bool applyEffect;

		public override void AddItem(Mastery mastery)
		{
			base.AddItem(mastery);
			if (applyEffect)
				mastery.OnEquip();
		}

		public override void RemoveItem(Mastery mastery)
		{
			if (applyEffect)
				if (RuntimeItems.Contains(mastery))
					mastery.OnRemove();

			base.RemoveItem(mastery);
		}
	}
}