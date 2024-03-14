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
	public class UIManager : Singleton<UIManager>
	{
		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider bgmVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;

		[SerializeField] private GameObject settingPanel;

		[SerializeField] private UIFloatingText uiDamage;

		[SerializeField] private Animator transitionAnimator;
		[SerializeField] private UIPopup popup;

		[SerializeField] private Toggle framerateToggle;

		public UITab Tab { get; private set; }
		public CutSceneModule CutSceneModule { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			Tab = FindObjectOfType<UITab>(true);
			CutSceneModule = FindObjectOfType<CutSceneModule>(true);
		}

		private void Start()
		{
			InitVolumeSliderValue();
		}

		private void Update()
		{
			SOManager.Instance.IsMouseOnUI.RuntimeValue = EventSystem.current.IsPointerOverGameObject();
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

		public void ToggleFramerate()
		{
			Application.targetFrameRate = framerateToggle.isOn ? 60 : 30;
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
	}
}