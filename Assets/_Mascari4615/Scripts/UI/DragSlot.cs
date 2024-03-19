using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
	public class DragSlot : MonoBehaviour
	{
		public bool isHoldingSomething => _holdingSlot != null;
		public UIItemSlot HoldingSlot => _holdingSlot;

		static public DragSlot instance;
		private UIItemSlot _holdingSlot;
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private Image specialThingImage;

		private void Awake()
		{
			instance = this;
		}

		public void SetSlot(UIItemSlot slot)
		{
			_holdingSlot = slot;

			if (slot == null)
				return;

			specialThingImage.sprite = slot.Artifact.Thumbnail;
			SetColor(1);
		}

		public void SetColor(float _alpha)
		{
			canvasGroup.alpha = _alpha;
		}
	}
}