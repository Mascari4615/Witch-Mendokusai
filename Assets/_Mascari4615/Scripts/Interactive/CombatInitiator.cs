using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class CombatInitiator : MonoBehaviour
	{
		[SerializeField] private Dungeon dungeon;

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Initiating();
			}
		}

		public void Initiating()
		{
			DungeonManager.Instance.StartDungeon(dungeon);
		}
	}
}