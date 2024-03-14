using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestSlot : UISlot
	{
		[SerializeField] private GameObject completeEffect;
		private bool complete = false;

		public void UpdateUI()
		{
			complete = (Artifact as Quest).Complete;
			completeEffect.SetActive(complete);
		}
	}
}