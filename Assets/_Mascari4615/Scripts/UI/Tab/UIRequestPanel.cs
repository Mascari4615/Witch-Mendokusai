using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIRequestPanel : UIPanel
	{
		private UIQuestBuffer questDataBufferUI;

		public override void Init()
		{
			questDataBufferUI = GetComponentInChildren<UIQuestBuffer>(true);
			questDataBufferUI.Init();
		}

		public override void UpdateUI()
		{
			questDataBufferUI.UpdateUI();
		}
	}
}
