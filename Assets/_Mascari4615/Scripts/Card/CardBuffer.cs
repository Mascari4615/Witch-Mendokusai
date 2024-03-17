using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(CardBuffer), menuName = "GameSystem/DataBuffer/" + nameof(Card))]
	public class CardBuffer : DataBuffer<Card>
	{
		[SerializeField] private bool applyEffect;

		public override void AddItem(Card card)
		{
			base.AddItem(card);
			if (applyEffect)
				card.OnEquip();
		}

		public override void RemoveItem(Card card)
		{
			if (applyEffect)
				if (RuntimeItems.Contains(card))
					card.OnRemove();

			base.RemoveItem(card);
		}
	}
}