using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.NodeHelper;
using UnityEngine.AI;

namespace Mascari4615
{
	public class BT_Idle : BTRunner
	{
		public BT_Idle(float randomMoveDistance = 10)
		{
			this.randomMoveDistance = randomMoveDistance;
		}

		private float randomMoveDistance;
		
		private Vector3 moveDest = Vector3.zero;

		protected override Node MakeNode()
		{
			return
				Sequence
				(
					Wait(3),
					Action(SetDestinationRandom),
					Action(MoveToDestination),
					Action(UpdateSpriteFlip)
				);
		}

		private void SetDestinationRandom()
		{
			Vector3 random = UnityEngine.Random.insideUnitCircle * randomMoveDistance;
			random.y = 0;
			moveDest = unitObject.transform.position + random;
		}

		private void MoveToDestination()
		{
			NavMeshAgent agent = unitObject.NavMeshAgent;

			Vector3 dir = (moveDest - unitObject.transform.position).normalized;
			agent.destination = unitObject.transform.position + dir;
		}

		private void UpdateSpriteFlip()
		{
			unitObject.SpriteRenderer.flipX = IsPlayerOnLeft();
		}

		protected bool IsPlayerOnLeft()
		{
			return Camera.main.WorldToViewportPoint(unitObject.transform.position).x > .5f;
		}
	}
}