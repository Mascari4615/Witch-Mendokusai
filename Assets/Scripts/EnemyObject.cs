using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class EnemyObject : UnitObject, IHitable
	{
		[SerializeField] private Inventory inventory;
		public GameEvent OnEnemyDied;

		[SerializeField] private EnemyObjectVariable lastHitEnemyObject;

		protected override void OnReceiveAttack(int damage)
		{
			base.OnReceiveAttack(damage);
			CombatManager.Instance.PopDamage(transform.position + Vector3.forward * 1, damage);

			if (!IsAlive)
			{
				return;
			}

			lastHitEnemyObject.RuntimeValue = this;
			hpBar.localScale = new Vector3((float)CurHp / unitData.MaxHp, 1, 1);

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

			if (flashRoutine != null)
			{
				StopCoroutine(flashRoutine);
			}

			flashRoutine = StartCoroutine(FlashRoutine());

			RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);

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
			DropLoot();

			OnEnemyDied?.Raise();

			// RuntimeManager.PlayOneShot(hurtSFX, transform.position);
			StopAllCoroutines();

			/*if (DataManager.Instance.CurGameData.killedOnceMonster[ID] == false)
            {
                if (Collection<>.Instance != null)
                {
                    Collection.Instance.Collect(this);
                }

                DataManager.Instance.CurGameData.killedOnceMonster[ID] = true;
                DataManager.Instance.SaveGameData();
            }*/

			//  RuntimeManager.PlayOneShot(collapseSFX, transform.position);

			// collider2D.enabled = false;
			// Rigidbody2D.velocity = Vector2.zero;
			// Rigidbody2D.bodyType = RigidbodyType2D.Static;
			// Animator.SetTrigger("COLLAPSE");
			GameManager.Instance.EnemyDataBuffer.Remove(gameObject);

			/*
            if (StageManager.Instance.CurrentRoom is NormalRoom)
            {
                onMonsterCollapse.Raise(transform);
                int randCount = Random.Range(0, 5 + 1);
                for (int i = 0; i < randCount; i++)
                {
                    ObjectManager.Instance.PopObject("ExpOrb", transform);
                }

                randCount = Random.Range(0, 5 + 1);
                for (int i = 0; i < randCount; i++)
                {
                    ObjectManager.Instance.PopObject("Goldu", transform);
                }
            }

            ObjectManager.Instance.PopObject("LevelUpEffect", transform);*/

			gameObject.SetActive(false);
		}

		protected virtual void DropLoot()
		{
			Probability<ItemData> probability = new();
			foreach (var item in (unitData as Enemy).Loots)
				probability.Add(item.Artifact as ItemData, item.Percentage);

			var newItem = probability.Get();
			inventory.Add(newItem);

			//
			GameObject exp = ObjectManager.Instance.PopObject(expPrefab);
			exp.transform.position = transform.position;
			exp.gameObject.SetActive(true);
		}

		[SerializeField] private GameObject expPrefab;

		private IEnumerator FlashRoutine()
		{
			spriteRenderer.material.SetFloat("_Emission", 1);
			yield return new WaitForSeconds(.1f);
			spriteRenderer.material.SetFloat("_Emission", 0);
			flashRoutine = null;
		}

		protected Vector3 GetRot()
		{
			return new Vector3(0, 0,
				(Mathf.Atan2(PlayerController.Instance.transform.position.y - (transform.position.y + 0.8f),
					PlayerController.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg) - 90);
		}

		protected Vector3 GetDirection()
		{
			return (PlayerController.Instance.transform.position - transform.position).normalized;
		}

		protected bool IsPlayerRight()
		{
			return PlayerController.Instance.transform.position.x > transform.position.x;
		}

		private Coroutine flashRoutine;
		[SerializeField] private Transform hpBar;

		protected override void Awake()
		{
			base.Awake();
			// Animator = GetComponent<Animator>();
			// Rigidbody2D = GetComponent<Rigidbody2D>();
			// collider2D = GetComponent<Collider2D>();

			/*collapseSFX = this is BossMonster
                ? $"event:/SFX/Monster/Boss_{ID}_Collapse"
                : $"event:/SFX/Monster/{ID}_Collapse";
            hurtSFX = this is BossMonster ? $"event:/SFX/Monster/Boss_{ID}_Hurt" : $"event:/SFX/Monster/{ID}_Hurt";*/
		}

		protected virtual void OnEnable()
		{
			// collider2D.enabled = true;
			// Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
			// Rigidbody2D.velocity = Vector3.zero;
			spriteRenderer.sharedMaterial = unitData.Material;
			GameManager.Instance?.EnemyDataBuffer.Add(gameObject);
			hpBar.localScale = Vector3.one;
			// Animator.SetTrigger("AWAKE");
		}

		protected virtual void OnDisable()
		{
			GameManager.Instance?.EnemyDataBuffer.Remove(gameObject);
			StopAllCoroutines();
		}
	}
}