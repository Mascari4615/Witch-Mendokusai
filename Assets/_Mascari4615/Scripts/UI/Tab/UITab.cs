using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UITab : MonoBehaviour
	{
		private int curPanelIndex = 0;
		private CanvasGroup canvasGroup;
		private UIPanel[] tabPanels;
		
		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			tabPanels = GetComponentsInChildren<UIPanel>(true);

			foreach (var tabPanel in tabPanels)
				tabPanel.Init();
		}

		private void Start()
		{
			OpenTabMenu(curPanelIndex);
		}

		public void OpenTabMenu(int newPanelIndex)
		{
			// Debug.Log($"{nameof(OpenTabMenu)}, {menuType}");
			curPanelIndex = newPanelIndex;

			for (var i = 0; i < tabPanels.Length; i++)
				tabPanels[i].gameObject.SetActive(i == curPanelIndex);
			tabPanels[curPanelIndex].UpdateUI();
		}

		public void ToggleTabMenu()
		{
			// Debug.Log($"{nameof(ToggleTabMenu)}");
			canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;
			canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
			canvasGroup.interactable = canvasGroup.alpha > 0;
		}
	}
}