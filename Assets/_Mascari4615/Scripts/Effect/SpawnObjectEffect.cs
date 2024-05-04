using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "SpawnObjectEffect", menuName = "Effect/SpawnObjectEffect")]
	public class SpawnObjectEffect : Effect
	{
		private GameObject instance;
		[SerializeField] private GameObject targetObject;

		public override void Apply()
		{
			if (instance != null)
			{
				instance.SetActive(false);
				instance = null;
			}

			instance = ObjectPoolManager.Instance.Spawn(targetObject);
			instance.SetActive(true);
		}

		public override void Cancle()
		{
			if (instance == null)
				return;

			instance.SetActive(false);
		}
	}
}