using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class TimeManager : Singleton<TimeManager>
	{
		// https://twitter.com/BinaryImpactG/status/1686306273061482496
		// Mathf.Epsilon

		public bool Paused => SOManager.Instance.IsPaused.RuntimeValue;
		public static float LocalTimeScale = 1f;
		public static float DeltaTime
		{
			get
			{
				return Time.deltaTime * LocalTimeScale;
			}
		}
		public static bool IsPaused
		{
			get
			{
				return LocalTimeScale == 0f;
			}
		}

		public static float TimeScale
		{
			get
			{
				return Time.timeScale * LocalTimeScale;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			SOManager.Instance.IsPaused.GameEvent.AddCallback(UpdateTimeScale);
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

		public float slowFactor = 0.05f;
		public float slowTime = .5f;
		public float returnSpeed = 4f;

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