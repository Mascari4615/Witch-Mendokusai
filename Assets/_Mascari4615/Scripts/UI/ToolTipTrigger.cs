using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Mascari4615
{
	public class ToolTipTrigger : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
	{
		public ToolTip targetToolTip;
		[SerializeField] private bool isSpecialThings = true;
		private Artifact _artifact;

		private Sprite sprite;
		private string header;
		private string description;

		private bool isShowingThis = false;

		public void SetToolTip(Artifact artifact) =>
			_artifact = artifact;

		public void SetToolTip(Sprite _sprite, string _name, string _description)
		{
			sprite = _sprite;
			header = _name;
			description = _description;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (isSpecialThings && _artifact == null)
				return;

			if (isSpecialThings)
				ToolTipModule.Instance.Show(_artifact);
			else
				ToolTipModule.Instance.Show(sprite, header, description);

			isShowingThis = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ToolTipModule.Instance.Hide();
			isShowingThis = false;
		}

		private void OnDisable()
		{
			if (isShowingThis)
			{
				if (targetToolTip != null)
					targetToolTip.gameObject.SetActive(false);
				ToolTipModule.Instance.Hide();
			}
		}

		public void Click()
		{
			if (targetToolTip == null)
				return;

			if (_artifact == null)
				return;

			if (sprite == null && header == string.Empty && description == string.Empty)
				return;

			if (isSpecialThings)
				targetToolTip.SetToolTip(_artifact);
			else
				targetToolTip.SetToolTip(sprite, header, description);

			targetToolTip.gameObject.SetActive(true);
		}
	}
}