using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentFairy : SkillComponent
	{
		[SerializeField] private int originDamage = 2;
		[SerializeField] private float originCoolTime = 1.5f;
		[SerializeField] private GameObject bulletPrefab;
		[SerializeField] private GameObject fairyPrefab;
		[SerializeField] private float rotateSpeed = -30f;

		private readonly List<Transform> fairyTransforms = new();
		private float coolTime;
		private int damageBonus;

		private Coroutine loop;

		private UnitStat PlayerStat => Player.Instance.UnitStat;

		public override void InitContext(SkillObject skillObject)
		{
			fairyTransforms.Clear();
		}

		private void OnEnable()
		{
			UpdateFairy();
			UpdateDamageBonus();
			UpdateAttackSpeedBonus();

			loop = StartCoroutine(Loop());
		}

		private void OnDisable()
		{
			if (loop != null)
				StopCoroutine(loop);
		}

		private void Start()
		{
			PlayerStat.AddListener(UnitStatType.FAIRY_COUNT, UpdateFairy);
			PlayerStat.AddListener(UnitStatType.FAIRY_DAMAGE_BONUS, UpdateDamageBonus);
			PlayerStat.AddListener(UnitStatType.FAIRY_ATTACK_SPEED_BONUS, UpdateAttackSpeedBonus);
		}

		private void Update()
		{
			transform.position = Player.Instance.transform.position;
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
		}

		private IEnumerator Loop()
		{
			int fairyIndex = 0;
			while (true)
			{
				if (Player.Instance.AutoAimPos == Vector3.zero)
				{
					yield return new WaitForSeconds(.1f);
					continue;
				}

				fairyIndex = ++fairyIndex % fairyTransforms.Count;

				GameObject g = ObjectPoolManager.Instance.Spawn(bulletPrefab);

				Vector3 spawnPosition = fairyTransforms[fairyIndex].position;
				spawnPosition.y = 0;
				g.transform.position = spawnPosition;

				if (g.TryGetComponent(out SkillObject skillObject))
					skillObject.InitContext(Player.Instance.Object);

				if (g.TryGetComponent(out DamagingObject damagingObject))
					damagingObject.SetDamageBonus(damageBonus);

				g.SetActive(true);

				yield return new WaitForSeconds(coolTime / fairyTransforms.Count);
			}
		}

		private void UpdateFairy()
		{
			int fairyCount = 1 + PlayerStat[UnitStatType.FAIRY_COUNT];

			if (fairyTransforms.Count < fairyCount)
			{
				int diff = fairyCount - fairyTransforms.Count;
				for (int i = 0; i < diff; i++)
				{
					GameObject g = ObjectPoolManager.Instance.Spawn(fairyPrefab);
					g.transform.SetParent(transform);
					g.transform.localPosition = Vector3.zero;
					g.SetActive(true);

					fairyTransforms.Add(g.transform.GetChild(0).transform);
				}
			}

			float delta = 360f / fairyCount;
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).transform.localRotation = Quaternion.Euler(Vector3.up * (delta * i));
			}
		}

		private void UpdateDamageBonus()
		{
			damageBonus = PlayerStat[UnitStatType.FAIRY_DAMAGE_BONUS];
		}

		private void UpdateAttackSpeedBonus()
		{
			coolTime = originCoolTime * (1 - PlayerStat[UnitStatType.FAIRY_ATTACK_SPEED_BONUS] * .2f);
		}
	}
}