using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Mascari4615
{
	public enum MCanvasType
	{
		None,
		Dungeon,
	}

	public enum MPanelType
	{
		None,
		Tab,
		Setting,
		DungeonResult,
		NPC,
	}

	public class UIManager : Singleton<UIManager>
	{
		[SerializeField] private Animator transitionAnimator;

		public MCanvasType CurCanvas { get; private set; }
		private readonly Dictionary<MCanvasType, UIPanel> canvasUIs = new();

		public MPanelType CurOverlay{ get; private set; }
		private readonly Dictionary<MPanelType, UIPanel> overlayUIs = new();

		public UITab Tab { get; private set; }
		public CutSceneModule CutSceneModule { get; private set; }
		public UISetting Setting { get; private set; }
		private UIFloatingText damage;
		private UIPopup popup;
		public UIChat Chat { get; private set; }
		private UIDungeon dungeon;
		private UIDungeonResult dungeonResult;
		public UINPC Npc { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			Tab = FindObjectOfType<UITab>(true);
			CutSceneModule = FindObjectOfType<CutSceneModule>(true);
			damage = FindObjectOfType<UIFloatingText>(true);
			popup = FindObjectOfType<UIPopup>(true);
			Setting = FindObjectOfType<UISetting>(true);
			Chat = FindObjectOfType<UIChat>(true);
			dungeon = FindObjectOfType<UIDungeon>(true);
			dungeonResult = FindObjectOfType<UIDungeonResult>(true);
			Npc = FindObjectOfType<UINPC>(true);

			canvasUIs[MCanvasType.Dungeon] = FindObjectOfType<UIDungeon>(true);

			overlayUIs[MPanelType.Tab] = Tab;
			overlayUIs[MPanelType.Setting] = Setting;
			overlayUIs[MPanelType.DungeonResult] = dungeonResult;
			overlayUIs[MPanelType.NPC] = Npc;
		}

		private void Start()
		{
			foreach (UIPanel uiPanel in canvasUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			foreach (UIPanel uiPanel in overlayUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			SetCanvas(MCanvasType.None);
			SetOverlay(MPanelType.None);
		}

		private void Update()
		{
			GameManager.Instance.IsMouseOnUI = EventSystem.current.IsPointerOverGameObject();
		}

		public void PopDamage(Vector3 pos, int damge)
		{
			StartCoroutine(damage.AniTextUI(pos, TextType.Damage, damge.ToString()));
		}

		// 함수를 전달받아 처리
		public void Transition(Action actionDuringTransition)
		{
			// Debug.Log(nameof(Transition) + " " + actionDuringTransition);
			StartCoroutine(TransitionCoroutine(ActionToCoroutine(actionDuringTransition)));

			static IEnumerator ActionToCoroutine(Action action)
			{
				action();
				yield return null;
			}
		}

		public void Transition(IEnumerator corountineDuringTransition)
		{
			// Debug.Log(nameof(Transition) + " " + corountineDuringTransition);
			StartCoroutine(TransitionCoroutine(corountineDuringTransition));
		}

		private IEnumerator TransitionCoroutine(IEnumerator corountineDuringTransition)
		{
			TimeManager.Instance.Pause();
			// Debug.Log(nameof(TransitionCoroutine));
			transitionAnimator.SetTrigger("IN");

			AnimatorStateInfo animatorStateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
			float duration = animatorStateInfo.length / animatorStateInfo.speedMultiplier;

			yield return new WaitForSecondsRealtime(duration + .2f);
			yield return StartCoroutine(corountineDuringTransition);
			yield return new WaitForSecondsRealtime(.2f);

			transitionAnimator.SetTrigger("OUT");
			TimeManager.Instance.Resume();
		}

		public void Popup(DataSO dataSO)
		{
			popup.Popup(dataSO);
		}

		public void ToggleOverlayUI_Tab()
		{
			if (CurOverlay != MPanelType.None)
			{
				if (CurOverlay != MPanelType.Setting)
					SetOverlay(MPanelType.None);
			}
			else
			{
				SetOverlay(MPanelType.Tab);
			}
		}

		public void ToggleOverlayUI_Setting()
		{
			if (CurOverlay != MPanelType.None)
			{
				SetOverlay(MPanelType.None);
			}
			else
			{
				SetOverlay(MPanelType.Setting);
			}
		}

		public void SetOverlay(MPanelType overlayUI)
		{
			if (CurOverlay == overlayUI)
				return;

			if (CurOverlay != MPanelType.None)
				overlayUIs[CurOverlay].SetActive(false);

			CurOverlay = overlayUI;
			if (overlayUIs.TryGetValue(overlayUI, out var uiPanel))
			{
				uiPanel.SetActive(true);
				uiPanel.UpdateUI();
			}
		}

		public void SetCanvas(MCanvasType newCanvas)
		{			
			if (CurCanvas == newCanvas)
				return;

			if (CurCanvas != MCanvasType.None)
				canvasUIs[CurCanvas].SetActive(false);

			CurCanvas = newCanvas;
			if (CurCanvas == MCanvasType.None)
			{
				CameraManager.Instance.SetCamera(CameraType.Normal);
			}
			else
			{
				canvasUIs[CurCanvas].SetActive(true);
				canvasUIs[CurCanvas].UpdateUI();
			}
		}
	}
}