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
		private int amount = 1;

		public Sprite Sprite => iconImage.sprite;
		public bool HasItem => iconImage.sprite != null;

		[SerializeField] protected Artifact defaultArtifact;

		protected Button button;
		protected Image iconImage;
		protected TextMeshProUGUI nameText;
		protected TextMeshProUGUI countText;
		protected TextMeshProUGUI descriptionText;
		protected Image disableImage;

		private Action<UISlot> selectAction;

		private bool isInit = false;

		public void Init()
		{
			if (isInit)
				return;
			isInit = true;

			button = GetComponent<Button>();

			iconImage = transform.Find("[Image] IconBackground").Find("[Image] Icon").GetComponent<Image>();
			disableImage = transform.Find("[Image] Disable").GetComponent<Image>();
			
			nameText = transform.Find("[Text] Name").GetComponent<TextMeshProUGUI>();
			countText = transform.Find("[Text] Count").GetComponent<TextMeshProUGUI>();
			descriptionText = transform.Find("[Text] Description").GetComponent<TextMeshProUGUI>();

			ToolTipTrigger = GetComponent<ToolTipTrigger>();

			if (defaultArtifact != null)
				SetArtifact(defaultArtifact);
		}

		public void SetSlotIndex(int index) => Index = index;
		public virtual void SetArtifact(Artifact artifact, int amount = 1)
		{
			Init();

			Artifact = artifact;
			this.amount = amount;

			UpdateUI();
			if (ToolTipTrigger)
				ToolTipTrigger.SetToolTipContent(Artifact);
		}

		public void UpdateUI()
		{
			if (Artifact)
			{
				iconImage.sprite = Artifact.Thumbnail;
				iconImage.color = Color.white;

				nameText.text = Artifact.Name;
				countText.text = amount.ToString();
				descriptionText.text = Artifact.Description;
			}
			else
			{
				iconImage.sprite = null;
				iconImage.color = Color.white * 0;

				nameText.text = string.Empty;
				countText.text = string.Empty;
				descriptionText.text = string.Empty;
			}
		}

		public void SetDisable(bool isDisable)
		{
			if (button)
				button.interactable = !isDisable;
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