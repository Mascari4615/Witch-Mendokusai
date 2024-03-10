using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIItemEquipMsgElement : MonoBehaviour
	{
		[SerializeField] private MMF_Player _mmfPlayer;
		[SerializeField] private UISlot _slot;

		public void SetAndPop(ItemData item)
		{
			_slot.SetArtifact(item);
			_mmfPlayer.StopFeedbacks();
			_mmfPlayer.PlayFeedbacks();
		}
	}
}