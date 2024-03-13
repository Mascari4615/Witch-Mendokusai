using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIManager : Singleton<UIManager>
	{
		public enum TabMenuPanelType
		{
			None = -1,
			BookShelf = 0,
			Inventory,
			PotionCraft,
		}

		public CutSceneModule CutSceneModule => cutSceneModule;
		[SerializeField] private CutSceneModule cutSceneModule;

		[SerializeField] private GameObject[] canvasList;
		[SerializeField] private UIPanel[] tabPanels;
		private PlayerState _curCanvas = PlayerState.Peaceful;
		private TabMenuPanelType _curTabMenuPanel = TabMenuPanelType.None;

		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider bgmVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;

		[SerializeField] private GameObject settingPanel;
		[SerializeField] private BoolVariable IsPaused;

		[SerializeField] private Animator transitionAnimator;

		public void OpenCanvas(int canvasType) => OpenCanvas((PlayerState)canvasType);
		public void OpenCanvas(PlayerState canvasType)
		{
			Debug.Log($"{nameof(OpenCanvas)}, {canvasType}");
			_curCanvas = canvasType;

			for (var i = 0; i < canvasList.Length; i++)
				canvasList[i].gameObject.SetActive(i == (int)_curCanvas);
		}

		public void OpenTabMenu(int canvasType) => OpenTabMenu((TabMenuPanelType)canvasType);
		public void OpenTabMenu(TabMenuPanelType menuType)
		{
			// Debug.Log($"{nameof(OpenTabMenu)}, {menuType}");
			_curTabMenuPanel = menuType;

			for (var i = 0; i < tabPanels.Length; i++)
				tabPanels[i].gameObject.SetActive(i == (int)_curTabMenuPanel);

			if (_curTabMenuPanel != TabMenuPanelType.None)
				tabPanels[(int)_curTabMenuPanel].UpdateUI();
		}

		public void OpenShopPanel()
		{

		}

		protected override void Awake()
		{
			base.Awake();

			foreach (var tabPanel in tabPanels)
				tabPanel.Init();
		}

		private void Start()
		{
			OpenCanvas(PlayerState.Peaceful);
			OpenTabMenu(TabMenuPanelType.None);
			InitVolumeSliderValue();
			SetMenuActive(false);
		}

		private void InitVolumeSliderValue()
		{
			masterVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.BusType.Master);
			bgmVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.BusType.BGM);
			sfxVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.BusType.SFX);
		}

		public void UpdateVolume(int busType) => UpdateVolume((AudioManager.BusType)busType);
		public void UpdateVolume(AudioManager.BusType busType)
		{
			switch (busType)
			{
				case AudioManager.BusType.Master:
					AudioManager.Instance.SetVolume(AudioManager.BusType.Master, masterVolumeSlider.value);
					break;
				case AudioManager.BusType.BGM:
					AudioManager.Instance.SetVolume(AudioManager.BusType.BGM, bgmVolumeSlider.value);
					break;
				case AudioManager.BusType.SFX:
					AudioManager.Instance.SetVolume(AudioManager.BusType.SFX, sfxVolumeSlider.value);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(busType), busType, null);
			}
		}

		[SerializeField] private Toggle framerateToggle;

		public void ToggleFramerate()
		{
			Application.targetFrameRate = framerateToggle.isOn ? 60 : 30;
		}

		[SerializeField] private GameObject menuPanel;
		[SerializeField] private GameObject menuButton;

		public void ToggleMenu()
		{
			SetMenuActive(!menuPanel.activeSelf);
		}

		public void SetMenuActive(bool active)
		{
			menuPanel.SetActive(active);
			menuButton.transform.rotation = Quaternion.Euler(0, 0, active ? -180 : 0);
		}

		public void ToggleSetting()
		{
			if (settingPanel.activeSelf)
			{
				settingPanel.SetActive(false);
				TimeManager.Instance.Resume();
			}
			else
			{
				if (TimeManager.Instance.Paused)
					return;

				settingPanel.SetActive(true);
				TimeManager.Instance.Pause();
			}

			// settingPanel.SetActive(!settingPanel.activeSelf);
		}

		public void ToggleTabMenu()
		{
			if (_curTabMenuPanel != TabMenuPanelType.Inventory)
				OpenTabMenu(TabMenuPanelType.Inventory);
			else
				OpenTabMenu(TabMenuPanelType.None);
		}

		// 가상함수를 전달받아 처리
		public void Transition(Action actionDuringTransition)
		{
			StartCoroutine(TransitionCoroutine(ActionToCoroutine(actionDuringTransition)));
		}

		public void Transition(IEnumerator corountineDuringTransition)
		{
			StartCoroutine(TransitionCoroutine(corountineDuringTransition));
		}

		private IEnumerator TransitionCoroutine(IEnumerator corountineDuringTransition)
		{
			transitionAnimator.SetTrigger("IN");
			
			AnimatorStateInfo animatorStateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
			float duration = animatorStateInfo.length / animatorStateInfo.speedMultiplier;
	
			yield return new WaitForSecondsRealtime(duration + .2f);
			yield return StartCoroutine(corountineDuringTransition);
			yield return new WaitForSecondsRealtime(.2f);

			transitionAnimator.SetTrigger("OUT");
		}

		private IEnumerator ActionToCoroutine(Action action)
		{
			action();
			yield return null;
		}
	}
}