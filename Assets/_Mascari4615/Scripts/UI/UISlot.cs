using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Mascari4615
{
	public class SlotData
	{
		public DataSO DataSO { get; private set; }= null;
		public Sprite Sprite{ get; private set; } = null;
		public string Name { get; private set; }= "";
		public string Description { get; private set; }= "";

		public bool IsEmpty { get; private set; }

		public void SetData(Sprite sprite, string name, string description)
		{
			if (sprite != null || string.IsNullOrEmpty(name) == false)
			{
				DataSO = null;
				Sprite = sprite;
				Name = name;
				Description = description;

				IsEmpty = false;
			}
			else
			{
				Init();
			}
		}

		public void SetData(DataSO dataSO)
		{
			if (dataSO)
			{
				DataSO = dataSO;
				Sprite = dataSO.Sprite;
				Name = dataSO.Name;
				Description = dataSO.Description;

				IsEmpty = false;
			}
			else
			{
				Init();
			}
		}

		public void Init()
		{
			DataSO = null;
			Sprite = null;
			Name = "";
			Description = "";

			IsEmpty = true;
		}
	}

	public class UISlot : MonoBehaviour
	{
		public int Index { get; private set; }
		public ToolTipTrigger ToolTipTrigger { get; private set; }
		public SlotData Data { get; private set; }
		public int Amount { get; private set; }

		[SerializeField] protected DataSO defaultDataSO;

		protected Button button;
		protected Image iconImage;
		protected TextMeshProUGUI nameText;
		protected TextMeshProUGUI countText;
		protected TextMeshProUGUI descriptionText;
		protected Image disableImage;
		protected GameObject selectedBlock;

		private Action<UISlot> selectAction;

		private bool isInit = false;
		private bool isDisable = false;

		public DataSO DataSO => Data.DataSO;

		public virtual bool Init()
		{
			if (isInit)
				return false;
			isInit = true;

			button = GetComponent<Button>();
			if (button)
				button.onClick.AddListener(Select);

			iconImage = transform.Find("[Image] IconBackground").Find("[Image] Icon").GetComponent<Image>();
			disableImage = transform.Find("[Image] Disable").GetComponent<Image>();

			nameText = transform.Find("[Text] Name").GetComponent<TextMeshProUGUI>();
			countText = transform.Find("[Text] Count").GetComponent<TextMeshProUGUI>();
			descriptionText = transform.Find("[Text] Description").GetComponent<TextMeshProUGUI>();
			selectedBlock = transform.Find("[Block] Selected").gameObject;

			ToolTipTrigger = GetComponent<ToolTipTrigger>();

			Data = new();

			if (defaultDataSO != null)
				SetSlot(defaultDataSO);

			return true;
		}

		public void SetSlotIndex(int index) => Index = index;

		public void SetSlot(DataSO dataSO, int amount = 1)
			=> SetSlot_(() => Data.SetData(dataSO), amount);

		public void SetSlot(Sprite sprite, string name, string description, int amount = 1)
			=> SetSlot_(() => Data.SetData(sprite, name, description), amount);

		private void SetSlot_(Action action, int amount = 1)
		{
			Init();

			action?.Invoke();
			Amount = amount;

			if (ToolTipTrigger)
				ToolTipTrigger.SetToolTipContent(Data);

			UpdateUI();
		}

		public virtual void UpdateUI()
		{
			iconImage.sprite = Data.Sprite;
			iconImage.color = Data.Sprite == null ? Color.clear : Color.white;
			nameText.text = Data.Name;
			descriptionText.text = Data.Description;

			countText.text = Amount == 1 ? "" : Amount.ToString();
		}

		public void SetDisable(bool isDisable)
		{
			this.isDisable = isDisable;
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

			if (ToolTipTrigger)
				ToolTipTrigger.Click();
		}

		public void SetSelected(bool isSelect)
		{
			if (selectedBlock)
				selectedBlock.SetActive(isSelect);
		}
	}
}