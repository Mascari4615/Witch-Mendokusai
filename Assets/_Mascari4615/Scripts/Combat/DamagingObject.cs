using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public interface IHitable
	{
		public void ReceiveDamage(int damage);
	}

	public class DamagingObject : SkillComponent
	{
		[SerializeField] private int damage;
		[SerializeField] private bool disableWhenHit;

		[SerializeField] private bool usedByPlayer = false;
		private bool vaild = true;

		public void OnTriggerEnter(Collider other)
		{
			if (vaild == false)
				return;

			if (other.TryGetComponent(out IHitable hitable))
			{
				switch (hitable)
				{
					case MonsterObject when usedByPlayer:
					case PlayerObject when !usedByPlayer:
						// Debug.Log(nameof(OnTriggerEnter));
						hitable.ReceiveDamage(damage);
						if (disableWhenHit)
							TurnOff();
						break;
				}
			}
		}

		public override void InitContext(SkillObject skillObject)
		{
			usedByPlayer = skillObject.UsedByPlayer;
			vaild = true;
		}

		private void TurnOff()
		{
			vaild = false;
			gameObject.SetActive(false);
		}
	}
}