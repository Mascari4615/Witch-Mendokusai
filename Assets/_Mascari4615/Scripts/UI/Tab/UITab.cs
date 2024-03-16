using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UITab : UIPanel
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

		public override void UpdateUI(int[] someData = null)
		{
		}
	}
}