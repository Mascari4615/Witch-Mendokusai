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
		public CutSceneModule CutSceneModule => cutSceneModule;
		[SerializeField] private CutSceneModule cutSceneModule;

		[SerializeField] private GameObject[] canvasList;
		private PlayerState _curCanvas = PlayerState.Peaceful;

		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider bgmVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;

		[SerializeField] private GameObject settingPanel;

		[SerializeField] private UIFloatingText uiDamage;

		[SerializeField] private Animator transitionAnimator;
		[SerializeField] private UIPopup popup;
		public UITab Tab { get; private set;}

		public void OpenCanvas(int canvasType) => OpenCanvas((PlayerState)canvasType);
		public void OpenCanvas(PlayerState canvasType)
		{
			// Debug.Log($"{nameof(OpenCanvas)}, {canvasType}");
			_curCanvas = canvasType;

			for (var i = 0; i < canvasList.Length; i++)
				canvasList[i].gameObject.SetActive(i == (int)_curCanvas);
		}

		public void OpenShopPanel()
		{

		}

		protected override void Awake()
		{
			base.Awake();
			Tab = FindObjectOfType<UITab>();
		}

		private void Start()
		{
			OpenCanvas(PlayerState.Peaceful);
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

		public void PopDamage(Vector3 pos, int damge)
		{
			StartCoroutine(uiDamage.AniTextUI(pos, TextType.Damage, damge.ToString()));
		}

		// 가상함수를 전달받아 처리
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
			// Debug.Log(nameof(TransitionCoroutine));
			transitionAnimator.SetTrigger("IN");

			AnimatorStateInfo animatorStateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
			float duration = animatorStateInfo.length / animatorStateInfo.speedMultiplier;

			yield return new WaitForSecondsRealtime(duration + .2f);
			yield return StartCoroutine(corountineDuringTransition);
			yield return new WaitForSecondsRealtime(.2f);

			transitionAnimator.SetTrigger("OUT");
		}

		public void Popup(Artifact artifact)
		{
			popup.Popup(artifact);
		}
	}
}