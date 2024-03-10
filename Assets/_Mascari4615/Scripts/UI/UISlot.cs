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
		public Sprite Sprite => image.sprite;
		public bool HasItem => image.sprite != null;

		public ToolTipTrigger toolTipTrigger;
		public Artifact Artifact { get; private set; }

		[SerializeField] protected Artifact defaultArtifact;
		[SerializeField] protected Image image;
		[SerializeField] protected TextMeshProUGUI nameText;
		[SerializeField] protected TextMeshProUGUI countTextField;
		[SerializeField] protected TextMeshProUGUI descriptionText;
		[SerializeField] private Image disableImage;
		private Action<Artifact> selectAction;

		private void Awake()
		{
			/// Init();
		}

		public void Init()
		{
			if (defaultArtifact != null)
				SetArtifact(defaultArtifact);
		}

		public void SetSlotIndex(int index) => Index = index;
		public virtual void SetArtifact(Artifact artifact, int amount = 1)
		{
			// Debug.Log(nameof(Init));
			Artifact = artifact;

			toolTipTrigger?.SetToolTip(artifact);

			image.sprite = artifact?.Thumbnail;
			image.color = artifact != null ? Color.white : Color.white * 0;

			if (nameText != null)
				nameText.text = artifact?.Name;

			if (countTextField != null)
				countTextField.text = HasItem ? amount.ToString() : string.Empty;

			if (descriptionText != null)
				descriptionText.text = artifact?.Description;
		}

		public void SetDisable(bool isDisable)
		{
			disableImage.gameObject.SetActive(isDisable);
		}

		public void SetSelectAction(Action<Artifact> action)
		{
			selectAction = action;
		}

		public void Select()
		{
			selectAction?.Invoke(Artifact);
		}
	}
}