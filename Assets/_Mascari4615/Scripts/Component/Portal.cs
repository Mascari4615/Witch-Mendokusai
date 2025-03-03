using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class Portal : MonoBehaviour
	{
		[field: SerializeField] public Transform TpPos { get; private set; }
		[field: SerializeField] public Stage TargetStage { get; private set; }
		[field: SerializeField] private int targetPortalIndex = -1;

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
				StageManager.Instance.LoadStage(TargetStage, targetPortalIndex);
		}

		public void Active()
		{
			gameObject.layer = 0;
		}
	}
}