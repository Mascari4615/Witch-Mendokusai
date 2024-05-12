using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentHulahoop : SkillComponent
	{
		[SerializeField] private GameObject satellitePrefab;
		[SerializeField] private float originRotateSpeed = 3;

		private readonly List<DamagingObject> satellites = new();
		private float rotateSpeed;

		private Stat PlayerStat => Player.Instance.Stat;

		private void OnEnable()
		{
			UpdateSatellite();
			UpdateRotationSpeed();
			UpdateDamageBonus();
			UpdateSatelliteScale();
		}

		private void Start()
		{
			PlayerStat.AddListener(StatType.SATELLITE_COUNT, UpdateSatellite);
			PlayerStat.AddListener(StatType.SATELLITE_ROTATE_SPEED_BONUS, UpdateRotationSpeed);
			PlayerStat.AddListener(StatType.SATELLITE_DAMAGE_BONUS, UpdateDamageBonus);
			PlayerStat.AddListener(StatType.SATELLITE_SCALE_BONUS, UpdateSatelliteScale);
		}

		private void Update()
		{
			transform.position = Player.Instance.transform.position;
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
		}

		public override void InitContext(SkillObject skillObject)
		{
			satellites.Clear();
		}

		private void UpdateSatellite()
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

					DamagingObject damagingObject = g.GetComponentInChildren<DamagingObject>(true);
					if (damagingObject != null)
						satellites.Add(damagingObject);
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

		private void UpdateRotationSpeed()
		{
			rotateSpeed = originRotateSpeed * (1 + PlayerStat[StatType.SATELLITE_ROTATE_SPEED_BONUS] * .3f);
		}

		private void UpdateDamageBonus()
		{
			foreach (DamagingObject satellite in satellites)
				satellite.SetDamageBonus(PlayerStat[StatType.SATELLITE_DAMAGE_BONUS]);
		}

		private void UpdateSatelliteScale()
		{
			foreach (DamagingObject satellite in satellites)
				satellite.transform.localScale = Vector3.one * (1 + PlayerStat[StatType.SATELLITE_SCALE_BONUS] * .2f);
		}
	}
}