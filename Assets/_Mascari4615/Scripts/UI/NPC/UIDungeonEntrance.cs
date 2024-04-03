using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonEntrance : UINPCPanel
	{
		[SerializeField] private GameObject[] dungeonSelectButtons;
		private int curDungeonIndex = 0;
		private List<Dungeon> dungeons;

		[SerializeField] private TextMeshProUGUI dungeonNameText;
		[SerializeField] private TextMeshProUGUI dungeonDescriptionText;

		public override void SetNPC(NPCObject npc)
		{
			// Debug.Log($"SetNPC: {npc.Data.Dungeons.Count}");
			dungeons = npc.Data.Dungeons;
		}

		public override void UpdateUI()
		{
			for (int i = 0; i < dungeonSelectButtons.Length; i++)
				dungeonSelectButtons[i].SetActive(dungeons.Count > i);

			SelectDungeon(0);
		}

		public void SelectDungeon(int index)
		{
			curDungeonIndex = index;
			UpdateDungeonPanel();
		}

		private void UpdateDungeonPanel()
		{
			// Debug.Log($"UpdateDungeonPanel: {curDungeonIndex}, {dungeons.Count}");
			if (dungeons.Count <= curDungeonIndex)
			{
				Debug.LogWarning("Invalid Dungeon Index");
				return;
			}

			Dungeon curDungeon = dungeons[curDungeonIndex];

			dungeonNameText.text = curDungeon.Name;
			dungeonDescriptionText.text = curDungeon.Description;
		}

		public void EnterTheDungeon()
		{
			if (dungeons.Count <= curDungeonIndex)
			{
				Debug.LogWarning("Invalid Dungeon Index");
				return;
			}

			DungeonManager.Instance.StartDungeon(dungeons[curDungeonIndex]);
		}
	}
}