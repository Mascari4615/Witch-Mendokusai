using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(AddCardEffect), menuName = "Effect/" + nameof(AddCardEffect))]
	public class AddCardEffect : Effect
	{
		[SerializeField] private CardBuffer deck;
		[SerializeField] private Card targetCard;

		public override void Apply()
		{
			deck.Add(targetCard);
		}

		public override void Cancle()
		{
		}
	}
}