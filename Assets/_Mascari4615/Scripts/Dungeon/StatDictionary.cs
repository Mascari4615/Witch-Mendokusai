using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum Stat
	{
		PLAYER_EXP_COLLIDER_SCALE,
		SATELLITE_COUNT
	}

	[CreateAssetMenu(fileName = nameof(StatDictionary), menuName = "StatDictionary")]
	public class StatDictionary : ScriptableObject, ISerializationCallbackReceiver
	{
		[NonSerialized] private Dictionary<Stat, int> _dic = new();
		[SerializeField] private GameEvent gameEvent;

		public void OnAfterDeserialize()
		{
			_dic = new();
		}

		public void OnBeforeSerialize()
		{
		}

		public void SetStat(Stat stat, int value)
		{
			if (!_dic.ContainsKey(stat))
				_dic[stat] = 0;

			_dic[stat] = value;
			gameEvent.Raise();
		}

		public int GetStat(Stat stat)
		{
			return _dic.TryGetValue(stat, out int value) ? value : 0;
		}
	}
}