using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIMagicBookPanel : UIPanels
	{
		private ToolTip toolTip;

		public override void Init()
		{
			base.Init();

			toolTip = GetComponentInChildren<ToolTip>(true);

			foreach (UIChapter chapter in panels.Cast<UIChapter>())
				chapter.SetToolTip(toolTip);
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