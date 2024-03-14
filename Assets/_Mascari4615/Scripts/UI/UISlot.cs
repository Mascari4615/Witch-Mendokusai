using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Mascari4615
{
	public class UISlot : MonoBehaviour
	{
		public int Index { get; private set; }
		public ToolTipTrigger ToolTipTrigger { get; private set; }
		public Artifact Artifact { get; private set; }

		public Sprite Sprite => image.sprite;
		public bool HasItem => image.sprite != null;

		[SerializeField] protected Artifact defaultArtifact;
		[SerializeField] protected Image image;
		[SerializeField] protected TextMeshProUGUI nameText;
		[SerializeField] protected TextMeshProUGUI countTextField;
		[SerializeField] protected TextMeshProUGUI descriptionText;
		[SerializeField] private Image disableImage;
		private Action<UISlot> selectAction;

		private bool isInit = false;

		private void Awake()
		{
			Init();
		}

		public void Init()
		{
			if (isInit)
				return;

			if (defaultArtifact != null)
				SetArtifact(defaultArtifact);

			ToolTipTrigger = GetComponent<ToolTipTrigger>();

			isInit = true;
		}

		public void SetSlotIndex(int index) => Index = index;
		public virtual void SetArtifact(Artifact artifact, int amount = 1)
		{
			// Debug.Log(nameof(Init));
			Artifact = artifact;
			ToolTipTrigger?.SetToolTipContent(Artifact);

			image.sprite = Artifact?.Thumbnail;
			image.color = Artifact != null ? Color.white : Color.white * 0;

			if (nameText != null)
				nameText.text = Artifact?.Name;

			if (countTextField != null)
				countTextField.text = HasItem ? amount.ToString() : string.Empty;

			if (descriptionText != null)
				descriptionText.text = Artifact?.Description;
		}

		public void SetDisable(bool isDisable)
		{
			disableImage.gameObject.SetActive(isDisable);
		}

		public void SetSelectAction(Action<UISlot> action)
		{
			selectAction = action;
		}

		public void Select()
		{
			// Debug.Log("Select");
			selectAction?.Invoke(this);
		}
	}
}