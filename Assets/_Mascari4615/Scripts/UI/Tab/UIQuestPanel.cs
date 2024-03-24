using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestPanel : UIPanel
	{
		private UIQuestBuffer questBufferUI;

		public override void Init()
		{
			questBufferUI = GetComponentInChildren<UIQuestBuffer>(true);
			questBufferUI.Init();
		}

		public override void UpdateUI()
		{
			questBufferUI.UpdateUI();
		}
	}
}
