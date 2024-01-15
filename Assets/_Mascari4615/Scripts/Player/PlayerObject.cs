using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerObject : UnitObject, IHitable
	{
		[SerializeField] private Transform hpBar;
		private Coroutine invincibleRoutine = null;

		public override void ReceiveDamage(int damage)
		{
			if (invincibleRoutine != null)
				return;
			
			base.ReceiveDamage(damage);

			if (!IsAlive)
				return;

			SOManager.Instance.CurHp.RuntimeValue = CurHp;
			hpBar.localScale = new Vector3((float)CurHp / unitData.MaxHp, 1, 1);

			if (invincibleRoutine != null)
				StopCoroutine(invincibleRoutine);
			invincibleRoutine = StartCoroutine(InvincibleTime());

			// Rigidbody2D.velocity = Vector3.zero;

			/*
			ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>()
				.SetText(damage.ToString(), hitType);
			ObjectManager.Instance.PopObject("Effect_Hit",
				transform.position + (Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f));*/

			/*
			if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[36]))
			{
				if ((float)hp / MaxHp <= 0.1f * DataManager.Instance.wgItemInven.itemCountDic[36])
				{
					ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up)
						.GetComponent<AnimatedText>().SetText("처형", Color.red);
					RuntimeManager.PlayOneShot(hurtSFX, transform.position);
					StopAllCoroutines();
					Collapse();
					return;
				}
			}
			*/

			RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);
			SOManager.Instance.OnPlayerHit.Raise();

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
			SOManager.Instance.OnPlayerDied.Raise();
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

				spriteRenderer.material.SetFloat("_Emission", isWhite ? 1 : 0);
				yield return new WaitForSeconds(.1f);
			}

			invincibleRoutine = null;
		}
	}
}