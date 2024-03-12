using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.NodeHelper;
using UnityEngine.AI;

namespace Mascari4615
{
	public class BT_RangeAttack : BTRunner
	{
		public BT_RangeAttack(float attackRange = 5f)
		{
			this.attackRange = attackRange;
		}
		
		private readonly float attackRange;

		private Vector3 moveDest = Vector3.zero;

		protected override Node MakeNode()
		{
			return
				Selector
				(
					// 근처에 있으면 Player향해서 Move
					Sequence
					(
						Condition(IsPlayerFar),

						Action(SetDestinationPlayer),
						Action(MoveToDestination),
						Action(UpdateSpriteFlip)
					),

					Sequence
					(
						Condition(IsSkill0Ready),
						Action(UseSkill0)
					)
				);
		}

		private void SetDestinationPlayer()
		{
			moveDest = PlayerController.Instance.transform.position;
		}

		protected bool IsPlayerFar()
		{
			float distance = Vector3.Distance(PlayerController.Instance.transform.position, unitObject.transform.position);
			bool isPlayerFar = distance > attackRange;

			return isPlayerFar;
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
		
		protected bool IsSkill0Ready()
		{
			return unitObject.UnitSkillHandler.IsReady(0);
		}

		protected void UseSkill0()
		{
			unitObject.UseSkill(0);
		}
	}
}