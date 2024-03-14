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

		public void SetToolTipContent(Artifact artifact) =>
			SetToolTipContent(artifact.Thumbnail, artifact.Name, artifact.Description);

		public void SetToolTipContent(Sprite sprite, string header, string description)
		{
			image.sprite = sprite;
			nameText.text = header;
			nameText.color = Color.white;
			// headerField.color = specialThing is HasGrade ? DataManager.Instance.GetGradeColor((specialThing as HasGrade).grade) : Color.white;
			descriptionText.text = description;
			gradeText.text = "";
			// if (gradeText != null)
			//    gradeText.text = specialThing is HasGrade ? $"{(specialThing as HasGrade).grade} ����????" : "";
		}
	}
}