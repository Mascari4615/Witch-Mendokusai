using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UINPC : UIPanel
	{
		private CanvasGroup canvasGroup;
		[SerializeField] private GameObject buttonsParent;
		[SerializeField] private UISlot talkOption;
		[SerializeField] private UISlot[] options;
		[SerializeField] private UISlot[] questOptions;

		// [SerializeField] private RectTransform panelParent;
		// [SerializeField] private float panelMove = 300;

		public NPCType CurPanelType { get; private set; }
		private readonly Dictionary<NPCType, UINPCPanel> panelUIs = new();
		private NPCObject curNPC;

		private void SetPanel(NPCType newPanelType)
		{
			if (CurPanelType == newPanelType)
				return;

			if (CurPanelType == NPCType.None)
			{
				buttonsParent.SetActive(false);
			}
			else
			{
				panelUIs[CurPanelType].SetActive(false);
			}

			CurPanelType = newPanelType;
			if (CurPanelType == NPCType.None)
			{
				CameraManager.Instance.SetCamera(CameraType.Dialogue);
				buttonsParent.SetActive(true);
			}
			else
			{
				CameraManager.Instance.SetChatCamera();
				panelUIs[CurPanelType].SetActive(true);
				panelUIs[CurPanelType].SetNPC(curNPC);
				panelUIs[CurPanelType].UpdateUI();
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

			panelUIs[NPCType.Shop] = FindObjectOfType<UIShop>(true);
			panelUIs[NPCType.DungeonEntrance] = FindObjectOfType<UIDungeonEntrance>(true);
			panelUIs[NPCType.Pot] = FindObjectOfType<UIPot>(true);
			panelUIs[NPCType.Anvil] = FindObjectOfType<UIAnvil>(true);
			panelUIs[NPCType.Furnace] = FindObjectOfType<UIFurnace>(true);

			foreach (UIPanel uiPanel in panelUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			int slotIndex = 0;
			foreach (NPCType type in System.Enum.GetValues(typeof(NPCType)))
			{
				NPCType npcType = type;
				Debug.Log($"{npcType}");

				if (npcType == NPCType.None)
					continue;

				if (!panelUIs.ContainsKey(npcType))
					continue;

				if (npcType == NPCType.Quest)
					continue;

				UIPanel panel = panelUIs[npcType];

				options[slotIndex].SetSlotIndex(slotIndex);
				options[slotIndex].SetSlot(panel.PanelIcon, panel.Name, string.Empty);
				options[slotIndex].Init();
				options[slotIndex].SetClickAction((slot) => { SetPanel(npcType); });
				slotIndex++;

				Debug.Log($"{npcType} {slotIndex}");
			}

			SetPanel(NPCType.None);
		}

		public override void OnOpen()
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			NPC curNPCData = curNPC.UnitData as NPC;
			List<QuestSO> questDatas = curNPCData.QuestData;
			for (int i = 0; i < questOptions.Length; i++)
			{
				if (i < questDatas.Count)
				{
					QuestState state = QuestManager.Instance.GetQuestState(questDatas[i].ID);
					if (state == QuestState.Completed)
					{
						questOptions[i].gameObject.SetActive(false);
						continue;
					}

					questOptions[i].SetSlot(questDatas[i]);
					questOptions[i].gameObject.SetActive(true);
				}
				else
				{
					questOptions[i].gameObject.SetActive(false);
				}
			}

			NPCType npcType = curNPCData.AllTypes;
			options[0].gameObject.SetActive(npcType.HasFlag(NPCType.Shop));
			options[1].gameObject.SetActive(npcType.HasFlag(NPCType.DungeonEntrance));
			options[2].gameObject.SetActive(npcType.HasFlag(NPCType.Pot));
			options[3].gameObject.SetActive(npcType.HasFlag(NPCType.Anvil));
			options[4].gameObject.SetActive(npcType.HasFlag(NPCType.Furnace));

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

				SetPanel(NPCType.None);
				buttonsParent.SetActive(true);
				talkOption.Select();
			});
		}

		private void SelectQuest(int index)
		{
			NPC curNPCData = curNPC.UnitData as NPC;
			List<QuestSO> questDatas = curNPCData.QuestData;
			QuestSO questData = questDatas[index];

			QuestState state = QuestManager.Instance.GetQuestState(questData.ID);
			switch (state)
			{
				case QuestState.Locked:
					QuestManager.Instance.UnlockQuest(questData);
					QuestManager.Instance.AddQuest(new RuntimeQuest(questData));
					break;
				case QuestState.Unlocked:
					// TODO: 퀘스트 진행 중 대사 출력
					break;
				case QuestState.Completed:
				default:
					Debug.LogError($"Invalid Quest State : {state}");
					break;
			}

			// HACK:
			Invoke(nameof(Close), .1f);
		}

		public void Close()
		{
			UIManager.Instance.SetOverlay(MPanelType.None);
		}
	}
}