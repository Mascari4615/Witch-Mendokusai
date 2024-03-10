using UnityEngine;

namespace Mascari4615
{
	public class ToolTipModule : MonoBehaviour
	{
		public static ToolTipModule Instance { get; private set; }

		[SerializeField] private ToolTip toolTip;
		private float toolTipWidth;
		private float toolTipHeight;
		private const float ToolTipPadding = 30f;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}
			DontDestroyOnLoad(gameObject);
			Instance = this;

			RectTransform rectTransform = toolTip.GetComponent<RectTransform>();
			toolTipWidth = rectTransform.sizeDelta.x;
			toolTipHeight = rectTransform.sizeDelta.y;
		}

		public void Show(Sprite sprite, string header, string description)
		{
			toolTip.SetToolTip(sprite, header, description);
			toolTip.transform.position = GetVec();
			toolTip.gameObject.SetActive(true);
		}

		public void Show(Artifact artifact)
		{
			// Debug.Log("Show");
			toolTip.SetToolTip(artifact);
			toolTip.transform.position = GetVec();
			toolTip.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (toolTip.gameObject.activeSelf)
				toolTip.transform.position = GetVec();
		}

		private Vector3 GetVec()
		{
			return new Vector3(
				Mathf.Clamp(Input.mousePosition.x, toolTipWidth / 2 + ToolTipPadding, Screen.width - toolTipWidth / 2 - ToolTipPadding),
				Mathf.Clamp(Input.mousePosition.y + 40, ToolTipPadding, Screen.height - toolTipHeight - ToolTipPadding), 0);
		}

		public void Hide()
		{
			// Debug.Log("Hide");
			toolTip.gameObject.SetActive(false);
		}
	}
}