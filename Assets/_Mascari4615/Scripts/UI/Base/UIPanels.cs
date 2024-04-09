using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public abstract class UIPanels : UIPanel
	{
		[SerializeField] private Transform panelSelectButtonsParent;
		protected int curPanelIndex = 0;
		protected UIPanel[] panels;

		public override void Init()
		{
			panels = GetComponentsInChildren<UIPanel>(true).Where(panel => panel != this).ToArray();
			foreach (UIPanel panel in panels)
				panel.Init();

			UISlot[] panelSelectButtons = panelSelectButtonsParent.GetComponentsInChildren<UISlot>(true);
			for (int i = 0; i < panelSelectButtons.Length; i++)
			{
				if (i >= panels.Length)
				{
					panelSelectButtons[i].gameObject.SetActive(false);
					continue;
				}

				panelSelectButtons[i].SetSlotIndex(i);
				panelSelectButtons[i].Init();
				panelSelectButtons[i].SetClickAction((slot) => { OpenPanel(slot.Index); });
			}
		}

		public override void OnOpen()
		{
			OpenPanel(curPanelIndex);
		}

		public override void UpdateUI()
		{
		}

		public void OpenPanel(int newPanelIndex)
		{
			// Debug.Log($"{nameof(OpenTabMenu)}, {menuType}");
			curPanelIndex = newPanelIndex;

			if (panels != null && panels.Length > 0)
			{
				for (int i = 0; i < panels.Length; i++)
					panels[i].gameObject.SetActive(i == curPanelIndex);
				panels[curPanelIndex].UpdateUI();
			}
		}
	}
}