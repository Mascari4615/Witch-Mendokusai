using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerController : Singleton<PlayerController>
	{
		[field: SerializeField]	public PlayerInteraction PlayerInteraction { get; private set; }
		[field: SerializeField]	public PlayerObject PlayerObject { get; private set; }
		[field: SerializeField]	public GameObject ExpCollider { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			PlayerInteraction = new();
		}

		public void TryInteract()
		{
			if (SOManager.Instance.CanInteract.RuntimeValue)
			{
				PlayerInteraction.Interaction();
			}
		}

		public void TryUseSkill(int skillIndex)
		{
			if (SOManager.Instance.IsCooling.RuntimeValue)
				return;
			PlayerObject.UseSkill(skillIndex);
		}
	}
}