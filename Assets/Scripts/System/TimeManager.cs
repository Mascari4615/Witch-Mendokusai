using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class TimeManager : Singleton<TimeManager>
	{
		[SerializeField] private BoolVariable IsPaused;
		public bool Paused => IsPaused.RuntimeValue;

		protected override void Awake()
		{
			base.Awake();
			IsPaused.GameEvent.AddCallback(UpdateTimeScale);
		}

		public void UpdateTimeScale()
		{
			Time.timeScale = IsPaused.RuntimeValue ? 0 : 1;
		}

		public void Pause()
		{
			IsPaused.RuntimeValue = true;
		}

		public void Resume()
		{
			IsPaused.RuntimeValue = false;
		}
	}
}