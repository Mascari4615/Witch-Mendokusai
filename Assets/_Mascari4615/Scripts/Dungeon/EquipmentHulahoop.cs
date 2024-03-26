using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class EquipmentHulahoop : MonoBehaviour
	{
		[SerializeField] private float rotateSpeed = 3;

		private Stat PlayerStat => PlayerController.Instance.PlayerObject.Stat;

		private void OnEnable()
		{
			Transform parent = PlayerController.Instance.transform;
			transform.SetParent(parent);
			transform.localPosition = Vector3.zero;

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
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
		}
	}
}