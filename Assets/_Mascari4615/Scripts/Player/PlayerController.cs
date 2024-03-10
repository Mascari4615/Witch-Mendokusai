using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerController : Singleton<PlayerController>
	{
		[SerializeField] private PlayerInteraction playerInteraction;
		[field: SerializeField]	public PlayerObject PlayerObject { get; private set; }
		public GameObject playerExpCollider;

		public void TeleportTo(Vector3 targetPos)
		{
			transform.position = targetPos;
		}

		public void PlayerButton0()
		{
			if (SOManager.Instance.CanInteract.RuntimeValue)
			{
				playerInteraction.Interaction();
			}
			else
			{
				PlayerObject.UseSkill(0);
			}
		}

		public void TryInteract()
		{
			if (SOManager.Instance.CanInteract.RuntimeValue)
			{
				playerInteraction.Interaction();
			}
		}

		public void TryUseSkill(int skillIndex)
		{
			if (SOManager.Instance.IsCooling.RuntimeValue)
				return;
			PlayerObject.UseSkill(skillIndex);
		}

		public void SetDoll(DollData dollData)
		{
			// 인형 선택시 호출?
		}

		private void UpdateDollData()
		{
			// TOOD : 뭐 이런 초기화 작업들?
			// maxHP.RuntimeValue = dolldat.MaxHp;
		}

		public void SetInteractionColliderLayer(int layer)
		{
			playerInteraction.SetLayer(layer);
		}
	}
}