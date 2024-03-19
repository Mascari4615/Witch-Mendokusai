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
		public WorkType workType;
		public int value;
	
		public float time;
		public float curTime = 0;

		public Work(WorkType workType, int value, float time)
		{
			this.workType = workType;
			this.value = value;
			this.time = time;
		}

		public void Tick(float tick)
		{
			curTime += tick;

			if (IsCompleted)
			{
				curTime = time;
			}
		}

		public bool IsCompleted => curTime >= time;
	}
}