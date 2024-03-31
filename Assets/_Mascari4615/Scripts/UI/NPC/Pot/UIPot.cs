using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class UIPot : UINPCPanel
	{
		private int curPanelIndex = 0;
		private UIPanel[] tabPanels;
		
		public void OpenTabMenu(int newPanelIndex)
		{
			// Debug.Log($"{nameof(OpenTabMenu)}, {menuType}");
			curPanelIndex = newPanelIndex;

			for (int i = 0; i < tabPanels.Length; i++)
			{
				tabPanels[i].gameObject.SetActive(i == curPanelIndex);
			}
			tabPanels[curPanelIndex].UpdateUI();
		}

		public override void Init()
		{
			tabPanels = GetComponentsInChildren<UIPanel>(true).Where(panel => panel != this).ToArray();
			foreach (UIPanel tabPanel in tabPanels)
			{
				tabPanel.Init();
			}
		}

		public override void OnOpen()
		{
			OpenTabMenu(curPanelIndex);
		}

		public override void UpdateUI()
		{
		}

		public override void SetNPC(NPCObject npc)
		{
		}
	}
}