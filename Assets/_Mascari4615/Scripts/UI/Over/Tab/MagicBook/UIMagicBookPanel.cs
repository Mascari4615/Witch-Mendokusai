using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIMagicBookPanel : UIPanels
	{
		private UIChapter[] chapters;
		private ToolTip toolTip;
		private int curChapterIndex = 0;

		public override void Init()
		{
			base.Init();

			chapters = GetComponentsInChildren<UIChapter>(true);
			toolTip = GetComponentInChildren<ToolTip>(true);

			foreach (UIChapter chapter in chapters)
			{
				chapter.Init();
				chapter.SetToolTip(toolTip);
			}
		}

		public override void UpdateUI()
		{
			base.UpdateUI();
			
			foreach (UIChapter chapter in chapters)
				chapter.UpdateUI();

			SelectChapter(curChapterIndex);
		}

		public void SelectChapter(int index)
		{
			curChapterIndex = index;

			for (int i = 0; i < chapters.Length; i++)
				chapters[i].gameObject.SetActive(i == curChapterIndex);

			chapters[index].UpdateUI();

			toolTip.gameObject.SetActive(false);
		}
	}
}