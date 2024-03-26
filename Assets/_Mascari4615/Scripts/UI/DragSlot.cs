using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
	public class DragSlot : Singleton<DragSlot>
	{
		private UIItemSlot holdingSlot;

		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private Image slotImage;

		public bool IsHolding => holdingSlot != null;
		public UIItemSlot HoldingSlot => holdingSlot;

		public void SetSlot(UIItemSlot slot)
		{
			holdingSlot = slot;

			if (slot == null)
				return;

			slotImage.sprite = slot.Artifact.Sprite;
			slotImage.color = slotImage.sprite == null ? Color.clear : Color.white;

			SetActive(true);
		}

		public void SetActive(bool active)
		{
			canvasGroup.alpha = active ? 1 : 0;
		}
	}
}