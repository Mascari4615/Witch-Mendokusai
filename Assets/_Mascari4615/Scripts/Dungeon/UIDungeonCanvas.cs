using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIDungeonCanvas : MonoBehaviour
	{
		[field: Header("_" + nameof(UIDungeonCanvas))]
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private TextMeshProUGUI timeText;
		[SerializeField] private TextMeshProUGUI difficultyText;
		[SerializeField] private Image difficultyCircle;

		public void SetActive(bool active)
		{
			canvasGroup.alpha = active ? 1 : 0;
		}

		public void UpdateUI()
		{
			UpdateDifficulty(DungeonManager.Instance.CurDifficulty);
			UpdateTime(DungeonManager.Instance.DungeonCurTime);
		}

		private void UpdateTime(TimeSpan timeSpan)
		{
			timeText.text = timeSpan.ToString(@"mm\:ss");
		}

		private void UpdateDifficulty(Difficulty curDifficulty)
		{
			if (curDifficulty == Difficulty.Hard)
			{
				difficultyCircle.fillAmount = 1;
			}
			else
			{
				difficultyCircle.fillAmount = (float)(DungeonManager.Instance.DungeonCurTime.TotalSeconds % 180f / 180f);
			}
			
			switch (curDifficulty)
			{
				case Difficulty.Easy:
					difficultyText.text = "쉬움";
					break;
				case Difficulty.Normal:
					difficultyText.text = "보통";
					break;
				case Difficulty.Hard:
					difficultyText.text = "어려움";
					break;
				default:
					break;
			}
		}
	}
}