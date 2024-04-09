using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIDungeonConstraint : UIPanel
	{
		// TODO: 던전 난이도/제약

		// 난이도 : 보통, 어려움, 매우 어려움
		// 하나의 UI

		// 제약 : ~

		// 슬롯으로?

		// TODO: 각 Dungeon마다 마지막으로 선택했던 제약 저장
		private Dungeon dungeon;
		private List<UISlot> constraintSlots;

		public override void Init()
		{
			constraintSlots = GetComponentsInChildren<UISlot>(true).ToList();
			
			for (int i = 0; i < constraintSlots.Count; i++)
			{
				if (CountOf<DungeonConstraint>() <= i)
				{
					constraintSlots[i].gameObject.SetActive(false);
					continue;
				}
				
				constraintSlots[i].gameObject.SetActive(true);
				constraintSlots[i].SetSlot(GetDungeonConstraint(i));
				constraintSlots[i].SetSlotIndex(i);
				constraintSlots[i].Init();
				constraintSlots[i].SetClickAction((UISlot slot) => ToggleConstraint(slot.Index));
			}
		}

		public void SetDungeon(Dungeon dungeon)
		{
			this.dungeon = dungeon;
		}

		public override void UpdateUI()
		{
			for (int i = 0; i < CountOf<DungeonConstraint>(); i++)
			{
				constraintSlots[i].SetDisable(GetDungeonConstraint(i).IsSelected == false);
			}
		}

		public void ToggleConstraint(int index)
		{
			GetDungeonConstraint(index).Toggle();
			UpdateUI();
		}
	}
}