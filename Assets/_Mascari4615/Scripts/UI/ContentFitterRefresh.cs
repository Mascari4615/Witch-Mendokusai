using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	// https://forum.unity.com/threads/content-size-fitter-refresh-problem.498536/
	public class ContentFitterRefresh : MonoBehaviour
	{
		private void Awake()
		{
			RefreshContentFitters();
		}

		public void RefreshContentFitters()
		{
			var rectTransform = (RectTransform)transform;
			RefreshContentFitter(rectTransform);
		}

		private void RefreshContentFitter(RectTransform transform)
		{
			if (transform == null || !transform.gameObject.activeSelf)
			{
				return;
			}

			foreach (RectTransform child in transform)
			{
				RefreshContentFitter(child);
			}

			var layoutGroup = transform.GetComponent<LayoutGroup>();
			var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
			if (layoutGroup != null)
			{
				layoutGroup.SetLayoutHorizontal();
				layoutGroup.SetLayoutVertical();
			}

			if (contentSizeFitter != null)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
			}
		}
	}
}