using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[Serializable]
	public struct StatInfo
	{
		public StatType stat;
		public int value;
	}

	[CreateAssetMenu(fileName = nameof(StatSO), menuName = nameof(StatSO))]
	public class StatSO : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField] private List<StatInfo> initStats = new();
		[field: NonSerialized] public Stat Stats { get; private set; }

		public void OnAfterDeserialize()
		{
			Stats = new();
			foreach (StatInfo statInfo in initStats)
				Stats[statInfo.stat] = statInfo.value;
		}

		public void OnBeforeSerialize()
		{
		}
	}
}