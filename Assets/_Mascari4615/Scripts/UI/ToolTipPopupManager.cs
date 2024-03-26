using System;
using UnityEngine;

namespace Mascari4615
{
	public class ToolTipPopupManager : Singleton<ToolTipPopupManager>
	{
		[SerializeField] private ToolTip popupToolTip;
		private float toolTipWidth;
		private float toolTipHeight;
		private const float ToolTipPadding = 30f;

		protected override void Awake()
		{
			base.Awake();

			RectTransform rectTransform = popupToolTip.GetComponent<RectTransform>();
			toolTipWidth = rectTransform.sizeDelta.x;
			toolTipHeight = rectTransform.sizeDelta.y;
		}

		public void Show(SlotData slotData)
		{
			popupToolTip.SetToolTipContent(slotData);
			popupToolTip.transform.position = GetVec();
			popupToolTip.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (popupToolTip.gameObject.activeSelf)
				popupToolTip.transform.position = GetVec();
		}

		private Vector3 GetVec()
		{
			return new Vector3(
				Mathf.Clamp(Input.mousePosition.x, toolTipWidth / 2 + ToolTipPadding, Screen.width - toolTipWidth / 2 - ToolTipPadding),
				Mathf.Clamp(Input.mousePosition.y + 40, ToolTipPadding, Screen.height - toolTipHeight - ToolTipPadding), 0);
		}

		public void Hide()
		{
			// Debug.Log("Hide");
			popupToolTip.gameObject.SetActive(false);
		}
	}
}