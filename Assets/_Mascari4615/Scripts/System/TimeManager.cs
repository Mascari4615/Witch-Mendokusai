using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	// https://twitter.com/BinaryImpactG/status/1686306273061482496
	// Mathf.Epsilon
	public class TimeManager : Singleton<TimeManager>
	{
		public const float Tick = 0.1f;
		public float slowFactor = 0.05f;
		public float slowTime = .5f;
		public float returnSpeed = 4f;

		private Action callback;
		private Coroutine timeLoop;

		protected override void Awake()
		{
			base.Awake();
			SOManager.Instance.IsPaused.GameEvent.AddCallback(UpdateTimeScale);
		}
		
		private void OnEnable()
		{
			timeLoop = StartCoroutine(UpdateTime());
		}
		
		private IEnumerator UpdateTime()
		{
			WaitForSeconds wait = new(Tick);
			while (true)
			{
				callback?.Invoke();
				yield return wait;
			}
		}

		public void AddCallback(Action callback)
		{
			if (this.callback != null)
			{
				foreach (Delegate existingCallback in this.callback.GetInvocationList())
					if (existingCallback.Equals(callback))
						return; // 이미 등록된 이벤트는 추가하지 않습니다.
			}

			this.callback += callback;
		}

		public void RemoveCallback(Action callback)
		{
			this.callback += callback;
		}

		public void UpdateTimeScale()
		{
			Time.timeScale = SOManager.Instance.IsPaused.RuntimeValue ? Mathf.Epsilon : 1;
		}

		public void Pause()
		{
			SOManager.Instance.IsPaused.RuntimeValue = true;
		}

		public void Resume()
		{
			SOManager.Instance.IsPaused.RuntimeValue = false;
		}

		[ContextMenu(nameof(DoSlowMotion))]
		public void DoSlowMotion()
		{
			if (slowMotion != null)
				StopCoroutine(slowMotion);
			slowMotion = StartCoroutine(SlowMotion());
		}

		private Coroutine slowMotion;

		// Timescale does not affect coroutines
		private IEnumerator SlowMotion()
		{
			yield return new WaitForSecondsRealtime(.05f);
			
			Time.timeScale = slowFactor;
			// Time.fixedDeltaTime = Time.timeScale * 0.02f;


			// Timescale affects WaitForSeconds
			// yield return new WaitForSeconds(slowTime);
			yield return new WaitForSecondsRealtime(slowTime);

			while (true)
			{
				Time.timeScale += Time.unscaledDeltaTime * returnSpeed;
				if (Time.timeScale > 1)
				{
					Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
					break;
				}

				yield return null;
			}
		}
	}
}