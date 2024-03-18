using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class Portal : MonoBehaviour, IInteractable
	{
		[field: SerializeField] public Transform TpPos { get; private set; }

		[SerializeField] private Stage targetStage;
		[SerializeField] private int targetPortalIndex;
	
		public void OnInteract()
		{
			StageManager.Instance.LoadStage(targetStage, targetPortalIndex);
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
				StageManager.Instance.LoadStage(targetStage, targetPortalIndex);
		}

		public void Active()
		{
			gameObject.layer = 0;
		}
	}
}