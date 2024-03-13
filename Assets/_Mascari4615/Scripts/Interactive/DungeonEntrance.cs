using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class DungeonEntrance : InteractiveObject
	{
		[SerializeField] private List<Dungeon> dungeonDatas;

		public override void Interact()
		{
			// Debug.Log(nameof(DungeonEntrance));
			DungeonManager.Instance.OpenDungeonEntranceUI(dungeonDatas);
		}
	}
}