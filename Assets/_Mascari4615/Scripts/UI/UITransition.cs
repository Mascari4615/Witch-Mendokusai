using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class UITransition : MonoBehaviour
	{
		private CanvasGroup canvasGroup;
		private Animator[] transitionAnimators;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			transitionAnimators = GetComponentsInChildren<Animator>(true);
		}

		private void Start()
		{
			canvasGroup.alpha = 1;
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
			// Debug.Log(nameof(TransitionCoroutine));
			
			// HACK:
			Animator transitionAnimator = transitionAnimators[Random.Range(0, transitionAnimators.Length)];

			TimeManager.Instance.Pause();
			canvasGroup.blocksRaycasts = true;
			transitionAnimator.SetTrigger("OUT");

			AnimatorStateInfo animatorStateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
			float duration = animatorStateInfo.length / animatorStateInfo.speedMultiplier;

			yield return new WaitForSecondsRealtime(duration + .2f);
			yield return StartCoroutine(corountineDuringTransition);
			yield return new WaitForSecondsRealtime(.2f);

			transitionAnimator.SetTrigger("IN");
			canvasGroup.blocksRaycasts = false;
			TimeManager.Instance.Resume();
		}
	}
}