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
		public int DollID { get; private set; }
		public WorkType WorkType { get; private set; }
		public int Value { get; private set; }
		public float Time { get; private set; }
		public float CurTime { get; private set; }

		public Work(int dollID, WorkType workType, int value, float time)
		{
			DollID = dollID;
			WorkType = workType;
			Value = value;
			Time = time;
		}

		public void Tick()
		{
			CurTime = Mathf.Clamp(CurTime + TimeManager.Tick, 0, Time);
		}
		public float GetProgress()
		{
			return CurTime / Time;
		}
		public bool IsCompleted()
		{
			return CurTime >= Time;
		}
	}
}