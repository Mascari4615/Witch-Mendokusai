using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	[Flags]
	public enum MNPCPanelType
	{
		None = 0,
		Shop = 1 << 0,
		DungeonEntrance = 1 << 1,
		Pot = 1 << 2,
		Upgrade = 1 << 3,
	}

	public class UINPC : UIPanel
	{
		private CanvasGroup canvasGroup;
		[SerializeField] private GameObject buttonsParent;
		[SerializeField] private UISlot exitOption;
		[SerializeField] private UISlot talkOption;
		[SerializeField] private UISlot[] options;
		[SerializeField] private UISlot[] questOptions;

		// [SerializeField] private RectTransform panelParent;
		// [SerializeField] private float panelMove = 300;

		public MNPCPanelType CurPanel { get; private set; }
		private readonly Dictionary<MNPCPanelType, UIPanel> panelUIs = new();
		private NPCObject curNPC;
		private NPC curNPCData;

		private void SetPanel(MNPCPanelType newPanel)
		{
			if (CurPanel == newPanel)
				return;

			if (CurPanel == MNPCPanelType.None)
			{
				buttonsParent.SetActive(false);
			}
			else
			{
				panelUIs[CurPanel].SetActive(false);
			}

			CurPanel = newPanel;
			if (CurPanel == MNPCPanelType.None)
			{
				CameraManager.Instance.SetCamera(CameraType.Dialogue);
				buttonsParent.SetActive(true);
			}
			else
			{
				CameraManager.Instance.SetChatCamera();
				panelUIs[CurPanel].SetActive(true);
				panelUIs[CurPanel].UpdateUI();
			}
		}

		public override void Init()
		{
			canvasGroup = GetComponent<CanvasGroup>();

			talkOption.SetSelectAction((slot) => { Talk(); });
			exitOption.SetSelectAction((slot) => { Exit(); });

			for (int i = 0; i < questOptions.Length; i++)
			{
				questOptions[i].SetSlotIndex(i);
				questOptions[i].Init();
				questOptions[i].SetSelectAction((slot) => { SelectQuest(slot.Index); });
			}

			for (int i = 0; i < options.Length; i++)
			{
				options[i].SetSlotIndex(i);
				options[i].Init();
				options[i].SetSelectAction((slot) => { SetPanel((MNPCPanelType)(1 << slot.Index)); });
			}

			panelUIs[MNPCPanelType.Shop] = FindObjectOfType<UIShop>(true);
			panelUIs[MNPCPanelType.DungeonEntrance] = FindObjectOfType<UIDungeonEntrance>(true);
			panelUIs[MNPCPanelType.Pot] = FindObjectOfType<UIPot>(true);

			foreach (UIPanel uiPanel in panelUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			SetPanel(MNPCPanelType.None);
		}

		public override void OnOpen()
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			List<QuestData> quests = curNPCData.Quests;
			for (int i = 0; i < questOptions.Length; i++)
			{
				if (i < quests.Count)
				{
					questOptions[i].SetSlot(quests[i]);
					questOptions[i].gameObject.SetActive(true);
				}
				else
				{
					questOptions[i].gameObject.SetActive(false);
				}
			}

			MNPCPanelType npcPanelType = curNPCData.PanelType;
			options[0].gameObject.SetActive(npcPanelType.HasFlag(MNPCPanelType.Shop));
			options[1].gameObject.SetActive(npcPanelType.HasFlag(MNPCPanelType.DungeonEntrance));
			options[2].gameObject.SetActive(npcPanelType.HasFlag(MNPCPanelType.Pot));

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
			curNPCData = curNPC.UnitData as NPC;
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

				SetPanel(MNPCPanelType.None);
				buttonsParent.SetActive(true);
			});
		}

		public void Exit()
		{
			UIManager.Instance.SetOverlay(MPanelType.None);
		}

		private void SelectQuest(int index)
		{
			QuestData questData = curNPCData.Quests[index];

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