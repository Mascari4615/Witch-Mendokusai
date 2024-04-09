using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonEntrance : UINPCPanel
	{
		[SerializeField] private Transform dungeonSelectButtonParent;
		[SerializeField] private UISlot dungeonSlot;

		private UISlot[] dungeonSelectButtons;
		private UIRewards rewardUI;
		private UIDungeonConstraint constraintUI;

		private int curDungeonIndex = 0;
		private List<Dungeon> dungeons;

		private Dungeon CurDungeon => dungeons[curDungeonIndex];

		public override void Init()
		{
			base.Init();

			dungeonSelectButtons = dungeonSelectButtonParent.GetComponentsInChildren<UISlot>(true);

			for (int i = 0; i < dungeonSelectButtons.Length; i++)
			{
				dungeonSelectButtons[i].SetSlotIndex(i);
				dungeonSelectButtons[i].Init();
				dungeonSelectButtons[i].SetClickAction((UISlot slot) => SelectDungeon(slot.Index));
			}

			rewardUI = GetComponentInChildren<UIRewards>(true);
			rewardUI.Init();

			constraintUI = GetComponentInChildren<UIDungeonConstraint>(true);
			constraintUI.Init();
		}

		public override void SetNPC(NPCObject npc)
		{
			dungeons = npc.Data.Dungeons;

			if (dungeons == null || dungeons.Count == 0)
				Debug.LogError("No Dungeon Data");
		}

		public override void UpdateUI()
		{
			for (int i = 0; i < dungeonSelectButtons.Length; i++)
				dungeonSelectButtons[i].gameObject.SetActive(dungeons.Count > i);

			SelectDungeon(0);
		}

		public void SelectDungeon(int index)
		{
			curDungeonIndex = index;
			UpdateDungeonPanel();
		}

		private void UpdateDungeonPanel()
		{
			dungeonSlot.SetSlot(CurDungeon);
			rewardUI.UpdateUI(CurDungeon.Rewards);
			constraintUI.UpdateUI();
		}

		public void EnterTheDungeon()
		{
			DungeonManager.Instance.StartDungeon(CurDungeon);
		}
	}
}