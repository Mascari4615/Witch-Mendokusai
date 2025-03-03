using System.Collections;
using FMODUnity;
using UnityEngine;
using static Mascari4615.MHelper;

namespace Mascari4615
{
	public class MonsterObject : UnitObject, IHitable
	{
		[Header("_" + nameof(MonsterObject))]
		[SerializeField] private GameObject hitEffectPrefab;
		[SerializeField] private GameObject dieEffectPrefab;

		private Coroutine flashRoutine;
		[SerializeField] private Transform hpBar;
		[SerializeField] private GameObject expPrefab;
		[SerializeField] private GameObject lootItemPrefab;

		public new Monster UnitData => base.UnitData as Monster;

		protected virtual void OnEnable()
		{
			SpriteRenderer.sharedMaterial = UnitData.Material;
			ObjectBufferManager.AddObject(ObjectType.Monster, gameObject);
			hpBar.localScale = Vector3.one;
		}

		protected virtual void OnDisable()
		{
			if (IsPlaying)
				ObjectBufferManager.RemoveObject(ObjectType.Monster, gameObject);
			StopAllCoroutines();
		}

		public override void ReceiveDamage(DamageInfo damageInfo)
		{
			base.ReceiveDamage(damageInfo);
			UIManager.Instance.PopDamage(damageInfo, transform.position + Vector3.forward * 1);

			SOManager.Instance.LastHitMonsterObject.RuntimeValue = this;
			hpBar.localScale = new Vector3((float)UnitStat[UnitStatType.HP_CUR] / UnitStat[UnitStatType.HP_MAX], 1, 1);

			GameObject hitEffect = ObjectPoolManager.Instance.Spawn(hitEffectPrefab);
			hitEffect.transform.position = transform.position + (Vector3.Normalize(Player.Instance.transform.position - transform.position) * .5f);
			hitEffect.SetActive(true);

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

			if (!IsAlive)
				return;

			if (flashRoutine != null)
			{
				StopCoroutine(flashRoutine);
			}

			flashRoutine = StartCoroutine(FlashRoutine());

			RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);

			switch (UnitStat[UnitStatType.HP_CUR])
			{
				case > 0:
					// Animator.SetTrigger("AHYA");
					break;
			}
		}

		protected override void OnDied()
		{
			base.OnDied();
			DropLoot();

			if (UnitData.Type == MonsterType.Boss)
				DataManager.Instance.DungeonStat[DungeonStatType.BOSS_KILL]++;
			DataManager.Instance.DungeonStat[DungeonStatType.MONSTER_KILL]++;

			RuntimeManager.PlayOneShot("event:/SFX/Monster/Die", transform.position);
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

			// Animator.SetTrigger("COLLAPSE");
			if (IsPlaying)
				ObjectBufferManager.RemoveObject(ObjectType.Monster, gameObject);

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
            }*/

			GameObject dieEffect = ObjectPoolManager.Instance.Spawn(dieEffectPrefab);
			dieEffect.transform.position = transform.position + (Vector3.Normalize(Player.Instance.transform.position - transform.position) * .5f);
			dieEffect.SetActive(true);

			gameObject.SetActive(false);
		}

		protected virtual void DropLoot()
		{
			Probability<ItemData> probability = new(shouldFill100Percent: true);
			foreach (DataSOWithPercentage item in UnitData.Loots)
			{
				if (item.DataSO == null)
				{
					Debug.LogError("DataSO is null");
					continue;
				}
				probability.Add(item.DataSO as ItemData, item.Percentage);
			}

			ItemData dropItem = probability.Get();
			if (dropItem != default)
			{
				GameObject lootItem = ObjectPoolManager.Instance.Spawn(lootItemPrefab);
				lootItem.transform.position = transform.position;
				lootItem.SetActive(true);
				lootItem.GetComponent<ItemObject>().Init(dropItem);
			}

			GameObject exp = ObjectPoolManager.Instance.Spawn(expPrefab);
			
			Vector3 pos = transform.position;
			pos += Vector3.up * 0.3f;
			pos += new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));

			exp.transform.position = pos;
			exp.SetActive(true);
		}

		private IEnumerator FlashRoutine()
		{
			SpriteRenderer.material.SetFloat("_Emission", 1);
			yield return new WaitForSeconds(.1f);
			SpriteRenderer.material.SetFloat("_Emission", 0);
			flashRoutine = null;
		}

		protected Vector3 GetRot()
		{
			return new Vector3(0, 0,
				(Mathf.Atan2(Player.Instance.transform.position.y - (transform.position.y + 0.8f),
					Player.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg) - 90);
		}

		protected Vector3 GetDirection()
		{
			return (Player.Instance.transform.position - transform.position).normalized;
		}

		protected bool IsPlayerRight()
		{
			return Player.Instance.transform.position.x > transform.position.x;
		}
	}
}