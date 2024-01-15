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
		public enum MenuPanelType
		{
			Home,
			BookShelf,
			Inventory,
			PotionCraft,
		}

		public CutSceneModule CutSceneModule => cutSceneModule;
		[SerializeField] private CutSceneModule cutSceneModule;

		[SerializeField] private GameObject[] canvasList;
		[SerializeField] private UIPanel[] menuPanelList;
		private PlayerState _curCanvas = PlayerState.Peaceful;
		private MenuPanelType _curMenuPanel = MenuPanelType.Home;

		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider bgmVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;

		[SerializeField] private GameObject settingPanel;
		[SerializeField] private BoolVariable IsPaused;

		public void OpenCanvas(int canvasType) => OpenCanvas((PlayerState)canvasType);
		public void OpenCanvas(PlayerState canvasType)
		{
			Debug.Log($"{nameof(OpenCanvas)}, {canvasType}");
			_curCanvas = canvasType;

			for (var i = 0; i < canvasList.Length; i++)
				canvasList[i].gameObject.SetActive(i == (int)_curCanvas);
		}

		public void OpenPanel(int canvasType) => OpenPanel((MenuPanelType)canvasType);
		public void OpenPanel(MenuPanelType menuType)
		{
			Debug.Log($"{nameof(OpenPanel)}, {menuType}");
			_curMenuPanel = menuType;

			for (var i = 0; i < menuPanelList.Length; i++)
				menuPanelList[i].gameObject.SetActive(i == (int)_curMenuPanel);
			menuPanelList[(int)_curMenuPanel].Init();

			SetMenuActive(false);
		}

		public void OpenShopPanel()
		{

		}

		private void Start()
		{
			OpenCanvas(PlayerState.Peaceful);
			OpenPanel(MenuPanelType.Home);
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

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				if (_curMenuPanel != MenuPanelType.Inventory)
					OpenPanel(MenuPanelType.Inventory);
				else
					OpenPanel(MenuPanelType.Home);
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				SetMenuActive(true);
			}
			else if (Input.GetKeyUp(KeyCode.Tab))
			{
				SetMenuActive(false);
			}

			if (Input.GetKeyDown(KeyCode.Escape))
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
		}
	}
}