using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
		[SerializeField] private GameObject[] buttons;

		// [SerializeField] private RectTransform panelParent;
		// [SerializeField] private float panelMove = 300;

		public MNPCPanelType CurPanel{ get; private set; }
		private readonly Dictionary<MNPCPanelType, UIPanel> panelUIs = new();

		public void SetPanel(int newPanelIndex)
		{
			SetPanel((MNPCPanelType)(1 << newPanelIndex));
		}

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

		private NPC curNPC;

		public override void Init()
		{
			canvasGroup = GetComponent<CanvasGroup>();

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
			
			buttons[0].SetActive(curNPC.PanelType.HasFlag(MNPCPanelType.Shop));
			buttons[1].SetActive(curNPC.PanelType.HasFlag(MNPCPanelType.DungeonEntrance));
			buttons[2].SetActive(curNPC.PanelType.HasFlag(MNPCPanelType.Pot));
			
			Talk();
		}

		public override void OnClose()
		{
			CameraManager.Instance.SetCamera(CameraType.Normal);
		}

		public override void UpdateUI()
		{
		}

		public void SetNPC(NPC npc)
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

				SetPanel(MNPCPanelType.None);
				buttonsParent.SetActive(true);
			});
		}

		public void Exit()
		{
			UIManager.Instance.SetOverlay(MPanelType.None);
		}
	}
}