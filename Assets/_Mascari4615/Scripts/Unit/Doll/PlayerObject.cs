using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class PlayerObject : UnitObject
	{
		private Coroutine invincibleRoutine = null;
		[SerializeField] private GameObject diedX;

		public void SetDoll(int dollID)
		{
			Init(GetDoll(dollID));
		}

		public override void Init(Unit unitData)
		{
			base.Init(unitData);
			GameManager.Instance.IsDied = false;
			diedX.SetActive(false);
		}

		public override void ReceiveDamage(DamageInfo damageInfo)
		{
			if (invincibleRoutine != null)
				return;
			
			if (!IsAlive)
				return;

			base.ReceiveDamage(damageInfo);

			RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);
			GameEventManager.Instance.Raise(GameEventType.OnPlayerHit);
			CameraManager.Instance.뽀삐뽀삐뽀();

			if (invincibleRoutine != null)
				StopCoroutine(invincibleRoutine);
			invincibleRoutine = StartCoroutine(InvincibleTime());

			/*
			ObjectManager.Instance.PopObject("Effect_Hit",
				transform.position + (Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f));*/

			switch (Stat[StatType.HP_CUR])
			{
				case > 0:
					// Animator.SetTrigger("AHYA");
					break;
			}
		}

		protected override void OnDied()
		{
			base.OnDied();
			GameManager.Instance.IsDied = true;
			GameEventManager.Instance.Raise(GameEventType.OnPlayerDied);
			TimeManager.Instance.DoSlowMotion();
			diedX.SetActive(true);
		}

		private IEnumerator InvincibleTime()
		{
			// TODO
			int invicibleTimeByDeciSec = (int)(SOManager.Instance.InvincibleTime.RuntimeValue * 10);
			bool isWhite = false;

			while (invicibleTimeByDeciSec > 0)
			{
				invicibleTimeByDeciSec--;
				isWhite = !isWhite;

				SpriteRenderer.material.SetFloat("_Emission", isWhite ? 1 : 0);
				yield return new WaitForSeconds(.1f);
			}

			SpriteRenderer.material.SetFloat("_Emission", 0);
			invincibleRoutine = null;
		}
	}
}