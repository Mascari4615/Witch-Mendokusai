using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonResult : MonoBehaviour
	{
		[SerializeField] private CanvasGroup canvasGroup;

		public void Init()
		{
			// ���� ��� ���� ������Ʈ
		}

		public void SetActive(bool active)
		{
			canvasGroup.alpha = active ? 1 : 0;
			canvasGroup.blocksRaycasts = active;
			canvasGroup.interactable = active;
		}

		public void Continue()
		{
			DungeonManager.Instance.Continue();
		}
	}
}