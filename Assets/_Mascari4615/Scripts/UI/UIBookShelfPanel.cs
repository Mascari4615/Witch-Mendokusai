using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIBookShelfPanel : UIPanel
	{
		private UIChapter[] chapters;
		private ToolTip toolTip;
		private int curChapterIndex = 0;

		private void Awake()
		{
			chapters = GetComponentsInChildren<UIChapter>(true);
			toolTip = GetComponentInChildren<ToolTip>(true);
		}

		public override void Init()
		{
			SelectChapter(curChapterIndex);

			foreach (var chapter in chapters)
				chapter.SetToolTip(toolTip);
		}

		public void SelectChapter(int index)
		{
			curChapterIndex = index;

			for (int i = 0; i < chapters.Length; i++)
				chapters[i].gameObject.SetActive(i == curChapterIndex);

			chapters[index].Init();

			toolTip.gameObject.SetActive(false);
		}
	}
}