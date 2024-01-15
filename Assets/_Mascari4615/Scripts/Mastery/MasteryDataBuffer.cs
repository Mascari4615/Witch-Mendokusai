using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(MasteryDataBuffer), menuName = "GameSystem/DataBuffer/Mastery")]
	public class MasteryDataBuffer : DataBuffer<Mastery>
	{
		[SerializeField] private bool useEffect;

		public override void AddItem(Mastery mastery)
		{
			base.AddItem(mastery);
			if (useEffect)
				mastery.OnEquip();
		}

		public override void RemoveItem(Mastery mastery)
		{
			if (useEffect)
				if (RuntimeItems.Contains(mastery))
					mastery.OnRemove();

			base.RemoveItem(mastery);
		}
	}
}