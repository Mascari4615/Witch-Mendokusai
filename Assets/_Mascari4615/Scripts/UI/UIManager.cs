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
	public enum OverlayUI
	{
		None,
		Tab,
		Setting,
		Shop,
		Chat,
		DungeonEntrance,
		DungeonResult,
	}

	public class UIManager : Singleton<UIManager>
	{
		[SerializeField] private Animator transitionAnimator;

		private GameContent curContentUI;
		private readonly Dictionary<GameContent, UIPanel> contentUIs = new();

		private OverlayUI curOverlayUI;
		private readonly Dictionary<OverlayUI, UIPanel> overlayUIs = new();

		public UITab Tab { get; private set; }
		public CutSceneModule CutSceneModule { get; private set; }
		public UISetting Setting { get; private set; }
		private UIFloatingText damage;
		private UIPopup popup;
		private UIShop shop;
		private UIChat chat;
		private UIDungeonEntrance dungeonEntrance;
		private UIDungeon dungeon;
		private UIDungeonResult dungeonResult;

		protected override void Awake()
		{
			base.Awake();

			Tab = FindObjectOfType<UITab>(true);
			CutSceneModule = FindObjectOfType<CutSceneModule>(true);
			damage = FindObjectOfType<UIFloatingText>(true);
			popup = FindObjectOfType<UIPopup>(true);
			Setting = FindObjectOfType<UISetting>(true);
			shop = FindObjectOfType<UIShop>(true);
			chat = FindObjectOfType<UIChat>(true);
			dungeonEntrance = FindObjectOfType<UIDungeonEntrance>(true);
			dungeon = FindObjectOfType<UIDungeon>(true);
			dungeonResult = FindObjectOfType<UIDungeonResult>(true);

			contentUIs[GameContent.Dungeon] = FindObjectOfType<UIDungeon>(true);

			overlayUIs[OverlayUI.Tab] = Tab;
			overlayUIs[OverlayUI.Setting] = Setting;
			overlayUIs[OverlayUI.Shop] = shop;
			overlayUIs[OverlayUI.Chat] = chat;
			overlayUIs[OverlayUI.DungeonEntrance] = dungeonEntrance;
			overlayUIs[OverlayUI.DungeonResult] = dungeonResult;
		}

		private void Start()
		{
			foreach (UIPanel uiPanel in contentUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			foreach (UIPanel uiPanel in overlayUIs.Values)
			{
				uiPanel.Init();
				uiPanel.SetActive(false);
			}

			SetOverlayUI(OverlayUI.None);
		}

		private void Update()
		{
			SOManager.Instance.IsMouseOnUI.RuntimeValue = EventSystem.current.IsPointerOverGameObject();
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

		public void Popup(Artifact artifact)
		{
			popup.Popup(artifact);
		}

		public void ToggleOverlayUI_Tab()
		{
			if (curOverlayUI != OverlayUI.None)
			{
				if (curOverlayUI != OverlayUI.Setting)
					SetOverlayUI(OverlayUI.None);
			}
			else
			{
				SetOverlayUI(OverlayUI.Tab);
			}
		}

		public void ToggleOverlayUI_Setting()
		{
			if (curOverlayUI != OverlayUI.None)
			{
				SetOverlayUI(OverlayUI.None);
			}
			else
			{
				SetOverlayUI(OverlayUI.Setting);
			}
		}

		public void SetOverlayUI(OverlayUI overlayUI, int[] someData = null)
		{
			if (curOverlayUI == overlayUI)
				return;

			if (curOverlayUI != OverlayUI.None)
				overlayUIs[curOverlayUI].SetActive(false);

			curOverlayUI = overlayUI;
			if (overlayUIs.TryGetValue(overlayUI, out var uiPanel))
			{
				uiPanel.SetActive(true);
				uiPanel.UpdateUI(someData);
			}
		}

		public void SetContentUI(GameContent gameContent, int[] someData = null)
		{
			if (curContentUI == gameContent)
				return;

			if (curContentUI != GameContent.None)
				contentUIs[curContentUI].SetActive(false);

			curContentUI = gameContent;
			if (contentUIs.TryGetValue(gameContent, out var uiPanel))
			{
				uiPanel.SetActive(true);
				uiPanel.UpdateUI(someData);
			}
		}
	}
}