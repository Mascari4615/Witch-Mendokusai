using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerObject : UnitObject, IHitable
	{
		private Coroutine invincibleRoutine = null;
		[SerializeField] private GameObject diedX;

		public override void Init(Unit unitData)
		{
			base.Init(unitData);

			SOManager.Instance.MaxHp.RuntimeValue = MaxHp;
			SOManager.Instance.CurHp.RuntimeValue = CurHp;
			SOManager.Instance.IsDied.RuntimeValue = false;

			diedX.SetActive(false);
		}

		public override void ReceiveDamage(int damage)
		{
			if (invincibleRoutine != null)
				return;
			
			if (!IsAlive)
				return;

			base.ReceiveDamage(damage);

			RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);
			SOManager.Instance.OnPlayerHit.Raise();
			SOManager.Instance.CurHp.RuntimeValue = CurHp;
			CameraManager.Instance.뽀삐뽀삐뽀();

			if (invincibleRoutine != null)
				StopCoroutine(invincibleRoutine);
			invincibleRoutine = StartCoroutine(InvincibleTime());

			/*
			ObjectManager.Instance.PopObject("Effect_Hit",
				transform.position + (Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f));*/

			switch (CurHp)
			{
				case > 0:
					// Animator.SetTrigger("AHYA");
					break;
			}
		}

		protected override void OnDie()
		{
			base.OnDie();
			SOManager.Instance.IsDied.RuntimeValue = true;
			SOManager.Instance.OnPlayerDied.Raise();
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

			invincibleRoutine = null;
		}

		public void UpdateCoolTime()
		{
			UnitSkillHandler.SetCoolTimeBonus(SOManager.Instance.CoolTimeBonus.RuntimeValue);
		}
	}
}