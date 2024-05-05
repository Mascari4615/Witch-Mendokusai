using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.NodeHelper;
using UnityEngine.AI;

namespace Mascari4615
{
	public class BT_MoveToPlayer : BTRunner
	{
		private Vector3 moveDest = Vector3.zero;
		private bool isSpriteLookLeft;

		public BT_MoveToPlayer(UnitObject unitObject) : base(unitObject)
		{
		}

		public void Init(bool isSpriteLookLeft = true)
		{
			this.isSpriteLookLeft = isSpriteLookLeft;
		}

		protected override Node MakeNode()
		{
			return
				Selector
				(
					Sequence
					(
						Action(SetDestinationPlayer),
						Action(MoveToDestination),
						Action(UpdateSpriteFlip)
					)
				);
		}

		private void SetDestinationPlayer()
		{
			moveDest = Player.Instance.transform.position;
		}

		private void MoveToDestination()
		{
			NavMeshAgent agent = unitObject.NavMeshAgent;

			Vector3 dir = (moveDest - unitObject.transform.position).normalized;
			agent.destination = unitObject.transform.position + dir;
		}

		private void UpdateSpriteFlip()
		{
			unitObject.SpriteRenderer.flipX = isSpriteLookLeft ? !IsPlayerOnLeft() : IsPlayerOnLeft();
		}

		protected bool IsPlayerOnLeft()
		{
			return Camera.main.WorldToViewportPoint(unitObject.transform.position).x > .5f;
		}
	}
}