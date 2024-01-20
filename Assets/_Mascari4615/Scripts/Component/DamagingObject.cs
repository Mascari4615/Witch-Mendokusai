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
		[field: Header("_" + nameof(DamagingObject))]
		[SerializeField] private int damage;

		[SerializeField] private bool useHitCount;
		[SerializeField] private int hitCount = 1;
		
		[SerializeField] private bool disableWhenInvaild;

		[SerializeField] private bool usedByPlayer = false;
		private bool vaild = true;
		private int curHitCount;

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
						if (useHitCount)
						{
							if (--curHitCount <= 0)
							{
								vaild = false;

								if (disableWhenInvaild)
									TurnOff();
							}
						}
						break;
				}
			}
		}

		public override void InitContext(SkillObject skillObject)
		{
			usedByPlayer = skillObject.UsedByPlayer;
			vaild = true;
			curHitCount = hitCount;
		}

		private void TurnOff()
		{
			vaild = false;
			gameObject.SetActive(false);
		}
	}
}