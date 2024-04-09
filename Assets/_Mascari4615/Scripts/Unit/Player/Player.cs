using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class Player : Singleton<Player>
	{
		public PlayerInteraction Interaction { get; private set; }
		public PlayerObject Object { get; private set; }
		[field: SerializeField]	public GameObject ExpCollider { get; private set; }

		public Stat Stat => Object.Stat;

		protected override void Awake()
		{
			base.Awake();
			Interaction = new();
			Object = GetComponent<PlayerObject>();
		}

		public void TryInteract()
		{
			if (SOManager.Instance.CanInteract.RuntimeValue)
			{
				Interaction.TryInteraction();
			}
		}

		public void TryUseSkill(int skillIndex)
		{
			if (SOManager.Instance.IsCooling.RuntimeValue)
				return;
			Object.UseSkill(skillIndex);
		}
	}
}