using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentHulahoop : SkillComponent
	{
		[SerializeField] private GameObject satellitePrefab;
		[SerializeField] private float defaultRotateSpeed = 3;

		private Stat PlayerStat => Player.Instance.Stat;

		public override void InitContext(SkillObject skillObject)
		{
		}

		private void OnEnable()
		{
			UpdateEquipment();
		}

		private void Start()
		{
			PlayerStat.AddListener(StatType.SATELLITE_COUNT, UpdateEquipment);
			UpdateEquipment();
		}

		public void UpdateEquipment()
		{
			int satelliteCount = 1 + PlayerStat[StatType.SATELLITE_COUNT];
			float delta = 360f / satelliteCount;

			if (transform.childCount < satelliteCount)
			{
				int diff = satelliteCount - transform.childCount;
				for (int i = 0; i < diff; i++)
				{
					GameObject g = ObjectPoolManager.Instance.Spawn(satellitePrefab);
					g.transform.SetParent(transform);
					g.transform.localPosition = Vector3.zero;
					g.SetActive(true);
				}
			}

			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(satelliteCount > i);
				transform.GetChild(i).transform.localRotation = Quaternion.Euler(Vector3.up * (delta * i));
			}

			Trail[] trails = transform.GetComponentsInChildren<Trail>(true);
			foreach (Trail trail in trails)
				trail.Clear();
		}

		private void Update()
		{
			transform.position = Player.Instance.transform.position;

			float speedBonus = 1 + (PlayerStat[StatType.SATELLITE_ROTATE_SPEED_BONUS] * .3f);
			transform.Rotate(0, defaultRotateSpeed * speedBonus * Time.deltaTime, 0);
		}
	}
}