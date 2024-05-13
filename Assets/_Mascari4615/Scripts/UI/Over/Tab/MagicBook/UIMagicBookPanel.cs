using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIMagicBookPanel : UIPanels
	{
		private ToolTip toolTip;
		private UIQuestToolTip questToolTip;

		public override void Init()
		{
			base.Init();

			toolTip = GetComponentInChildren<ToolTip>(true);
			
			questToolTip = GetComponentInChildren<UIQuestToolTip>(true);
			questToolTip.Init();

			foreach (UIChapter chapter in panels.Cast<UIChapter>())
				chapter.SetToolTip(toolTip, questToolTip);
		}

		public override void UpdateUI()
		{
			base.UpdateUI();

			foreach (UIChapter chapter in panels.Cast<UIChapter>())
				chapter.UpdateUI();
		}

		public override void OpenPanel(int newPanelIndex)
		{
			if (toolTip != null)
				toolTip.gameObject.SetActive(false);
			base.OpenPanel(newPanelIndex);
		}
	}
}