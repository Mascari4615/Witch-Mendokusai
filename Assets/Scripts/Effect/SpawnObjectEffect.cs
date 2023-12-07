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

		public override void OnEquip()
		{
			if (instance != null)
			{
				instance.SetActive(false);
				instance = null;
			}

			instance = ObjectManager.Instance.PopObject(targetObject);
			instance.SetActive(true);
		}

		public override void OnRemove()
		{
			if (instance == null)
				return;

			instance.SetActive(false);
		}
	}
}