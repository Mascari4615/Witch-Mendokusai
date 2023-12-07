using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatDictionary), menuName = "StatDictionary")]
	public class StatDictionary : ScriptableObject
	{
		[NonSerialized] private Dictionary<string, int> _dic = new();
		[SerializeField] private GameEvent gameEvent;

		public void OnAfterDeserialize()
		{
			_dic = new Dictionary<string, int>();
		}

		public void OnBeforeSerialize()
		{
		}

		public void SetStat(string stat, int value)
		{
			if (!_dic.ContainsKey(stat))
				_dic[stat] = 0;

			_dic[stat] += value;
			gameEvent.Raise();
		}

		public int GetStat(string stat)
		{
			return _dic.TryGetValue(stat, out int value) ? value : 0;
		}
	}
}