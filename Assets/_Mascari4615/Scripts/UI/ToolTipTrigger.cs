using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Mascari4615
{
	public class ToolTipTrigger : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
	{
		[field: SerializeField] public ToolTip ClickToolTip { get; private set; }
		[SerializeField] private bool isArtifact = true;
		[SerializeField] private bool usePopupToolTip = true;

		private Artifact _artifact;

		private Sprite sprite;
		private string header;
		private string description;

		private bool isPopupTooltipShowingThis = false;

		public void SetClickToolTip(ToolTip toolTip) => ClickToolTip = toolTip;

		public void SetToolTipContent(Artifact artifact) =>
			_artifact = artifact;

		public void SetToolTipContent(Sprite _sprite, string _name, string _description)
		{
			sprite = _sprite;
			header = _name;
			description = _description;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (usePopupToolTip == false)
				return;

			if (isArtifact && _artifact == null)
				return;

			if (isArtifact)
				ToolTipPopupManager.Instance.Show(_artifact);
			else
				ToolTipPopupManager.Instance.Show(sprite, header, description);

			isPopupTooltipShowingThis = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (usePopupToolTip == false)
				return;

			ToolTipPopupManager.Instance.Hide();
			isPopupTooltipShowingThis = false;
		}

		private void OnDisable()
		{
			if (isPopupTooltipShowingThis)
				ToolTipPopupManager.Instance.Hide();
		}

		public void Click()
		{
			if (ClickToolTip == null)
				return;

			if (isArtifact && _artifact == null)
				return;

			if (sprite == null && header == string.Empty && description == string.Empty)
				return;

			if (isArtifact)
				ClickToolTip.SetToolTipContent(_artifact);
			else
				ClickToolTip.SetToolTipContent(sprite, header, description);

			ClickToolTip.gameObject.SetActive(true);
		}
	}
}