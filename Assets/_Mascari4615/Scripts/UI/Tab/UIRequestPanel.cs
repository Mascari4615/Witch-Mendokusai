using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIRequestPanel : UIPanel
	{
		private ToolTip clickToolTip;
		private UIQuestDataBuffer questDataBufferUI;

		public override void Init()
		{
			clickToolTip = GetComponentInChildren<ToolTip>(true);
			questDataBufferUI = GetComponentInChildren<UIQuestDataBuffer>(true);

			questDataBufferUI.Init();
			foreach (UISlot slot in questDataBufferUI.Slots)
				slot.ToolTipTrigger.SetClickToolTip(clickToolTip);
		}

		public override void UpdateUI()
		{
			questDataBufferUI.UpdateUI();
		}
	}
}
