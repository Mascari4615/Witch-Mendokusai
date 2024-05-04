using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mascari4615
{
	// [Flags]
	public enum NPCPanelType
	{
		None = 0,
		Shop = 1 << 0,
		DungeonEntrance = 1 << 1,
		Pot = 1 << 2,
		Upgrade = 1 << 3,
		Quest = 1 << 4,
	}

	public class UINPC : UIPanel
	{
		private CanvasGroup canvasGroup;
		[SerializeField] private GameObject buttonsParent;
		[SerializeField] private UISlot talkOption;
		[SerializeField] private UISlot[] options;
		[SerializeField] private UISlot[] questOptions;

		// [SerializeField] private RectTransform panelParent;
		// [SerializeField] private float panelMove = 300;

		public NPCPanelType CurPanel { get; private set; }
		private readonly Dictionary<NPCPanelType, UINPCPanel> panelUIs = new();
		private NPCObject curNPC;

		private void SetPanel(NPCPanelType newPanel)
		{
			if (CurPanel == newPanel)
				return;

			if (CurPanel == NPCPanelType.None)
			{
				buttonsParent.SetActive(false);
			}
			else
			{
				panelUIs[CurPanel].SetActive(false);
			}

			CurPanel = newPanel;
			if (CurPanel == NPCPanelType.None)
			{
				CameraManager.Instance.SetCamera(CameraType.Dialogue);
				buttonsParent.SetActive(true);
			}
			else
			{
				CameraManager.Instance.SetChatCamera();
				panelUIs[CurPanel].SetActive(true);
				panelUIs[CurPanel].SetNPC(curNPC);
				panelUIs[CurPanel].UpdateUI();
			}
		}

		public override void Init()
		{
			canvasGroup = GetComponent<CanvasGroup>();

			talkOption.Init();
			talkOption.SetClickAction((slot) => { Talk(); });

			for (int i = 0; i < questOptions.Length; i++)
			{
				questOptions[i].SetSlotIndex(i);
				questOptions[i].Init();
				questOptions[i].SetClickAction((slot) => { SelectQuest(slot.Index); });
			}

			panelUIs[NPCPanelType.Shop] = FindObjectOfType<UIShop>(true);
			panelUIs[NPCPanelType.DungeonEntrance] = FindObjectOfType<UIDungeonEntrance>(true);
			panelUIs[NPCPanelType.Pot] = FindObjectOfType<UIPot>(true);

			foreach (UIPanel uiPanel in panelUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			for (int slotIndex = 0; slotIndex < options.Length; slotIndex++)
			{
				NPCPanelType p = (NPCPanelType)(1 << slotIndex);
				UIPanel panel = panelUIs[p];

				options[slotIndex].SetSlotIndex(slotIndex);
				options[slotIndex].SetSlot(panel.PanelIcon, panel.Name, string.Empty);
				options[slotIndex].Init();
				options[slotIndex].SetClickAction((slot) => { SetPanel((NPCPanelType)(1 << slot.Index)); });
			}

			SetPanel(NPCPanelType.None);
		}

		public override void OnOpen()
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			NPC curNPCData = curNPC.UnitData as NPC;
			List<QuestData> questDatas = curNPCData.QuestData;
			for (int i = 0; i < questOptions.Length; i++)
			{
				if (i < questDatas.Count)
				{
					questOptions[i].SetSlot(questDatas[i]);
					questOptions[i].gameObject.SetActive(true);
				}
				else
				{
					questOptions[i].gameObject.SetActive(false);
				}
			}

			NPCPanelType npcPanelType = curNPCData.AllPanelTypes;
			options[0].gameObject.SetActive(npcPanelType.HasFlag(NPCPanelType.Shop));
			options[1].gameObject.SetActive(npcPanelType.HasFlag(NPCPanelType.DungeonEntrance));
			options[2].gameObject.SetActive(npcPanelType.HasFlag(NPCPanelType.Pot));

			Talk();
		}

		public override void OnClose()
		{
			CameraManager.Instance.SetCamera(CameraType.Normal);
		}

		public override void UpdateUI()
		{
		}

		public void SetNPC(NPCObject npc)
		{
			curNPC = npc;
		}

		public void Talk()
		{
			buttonsParent.SetActive(false);
			CameraManager.Instance.SetCamera(CameraType.Dialogue);
			UIManager.Instance.Chat.StartChat(curNPC, () =>
			{
				// bool isNPCLeft = curNPC.transform.position.x < PlayerController.Instance.transform.position.x;
				// CameraManager.Instance.SetChatCamera(isNPCLeft);

				// Vector3 tmp = panelParent.anchoredPosition;
				// tmp.x = panelMove * (isNPCLeft ? -1 : 1);
				// panelParent.anchoredPosition = tmp;

				canvasGroup.alpha = 1;
				canvasGroup.interactable = true;
				canvasGroup.blocksRaycasts = true;

				SetPanel(NPCPanelType.None);
				buttonsParent.SetActive(true);
				talkOption.Select();
			});
		}

		private void SelectQuest(int index)
		{
			NPC curNPCData = curNPC.UnitData as NPC;
			List<QuestData> questDatas = curNPCData.QuestData;
			QuestData questData = questDatas[index];

			switch (questData.State)
			{
				case QuestDataState.Locked:
					questData.Unlock();
					DataManager.Instance.QuestManager.AddQuest(new Quest(questData));
					break;
				default:
					Debug.Log("Invalid Quest State");
					break;
			}

			UIManager.Instance.SetOverlay(MPanelType.None);
		}
	}
}