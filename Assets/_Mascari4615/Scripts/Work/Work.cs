using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum WorkType
	{
		CompleteQuest
	}

	public class Work
	{
		public WorkType WorkType { get; private set; }
		public int Value { get; private set; }

		private readonly float time;
		private float curTime = 0;

		public void Tick() => curTime = Mathf.Clamp(curTime + TimeManager.Tick, 0, time);
		public float Progress => curTime / time;
		public bool IsCompleted => curTime >= time;

		public Work(WorkType workType, int value, float time)
		{
			WorkType = workType;
			Value = value;
			this.time = time;
		}
	}
}