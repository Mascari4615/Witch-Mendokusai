using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mascari4615
{
	public class ToolTip : MonoBehaviour
	{
		[SerializeField] private Image image;
		[SerializeField] private TextMeshProUGUI nameText;
		[SerializeField] private TextMeshProUGUI descriptionText;
		[SerializeField] private TextMeshProUGUI gradeText;

		public void SetToolTipContent(SlotData slotData)
		{
			image.sprite = slotData.Sprite;
			image.color = slotData.Sprite == null ? Color.clear : Color.white;
			nameText.text = slotData.Name;
			descriptionText.text = slotData.Description;

			gradeText.text = "";
		}
	}
}