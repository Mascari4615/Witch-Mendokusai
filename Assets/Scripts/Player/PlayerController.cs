using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerController : Singleton<PlayerController>
	{
		[SerializeField] private BoolVariable canInteract;
		[SerializeField] private BoolVariable isPlayerButton0Down;
		[SerializeField] private PlayerInteraction playerInteraction;
		public PlayerObject PlayerObject => playerObject;
		[SerializeField] private PlayerObject playerObject;
		public GameObject playerExpCollider;

		public void TeleportTo(Vector3 targetPos)
		{
			transform.position = targetPos;
		}

		public void PlayerButton0()
		{
			if (canInteract.RuntimeValue)
			{
				playerInteraction.Interaction();
			}
			else
			{
				playerObject.UseSkill(0);
			}
		}

		private void Update()
		{
			if (canInteract.RuntimeValue && Input.GetKeyDown(KeyCode.Space))
			{
				playerInteraction.Interaction();
			}
			else if (isPlayerButton0Down.RuntimeValue || Input.GetKey(KeyCode.Space))
			{
				playerObject.UseSkill(0);
			}
		}

		[SerializeField] private IntVariable maxHP;

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