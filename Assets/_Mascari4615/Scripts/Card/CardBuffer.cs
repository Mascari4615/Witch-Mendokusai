using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(CardBuffer), menuName = "GameSystem/DataBuffer/" + nameof(Card))]
	public class CardBuffer : DataBuffer<Card>
	{
		[SerializeField] private bool applyEffect;

		public override void Add(Card card)
		{
			base.Add(card);
			if (applyEffect)
				card.OnEquip();
		}

		public override bool Remove(Card card)
		{
			if (applyEffect)
				if (RuntimeItems.Contains(card))
					card.OnRemove();

			return base.Remove(card);
		}
	}
}