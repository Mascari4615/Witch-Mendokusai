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

		public void Init()
		{
			image.enabled = false;
			nameText.text = "";
			descriptionText.text = "";
			gradeText.text = "";
		}

		public void SetToolTipContent(Artifact artifact)
		{
			if (artifact == null)
			{
				Init();
				return;
			}

			SetToolTipContent(artifact.Thumbnail, artifact.Name, artifact.Description);
		}

		public void SetToolTipContent(Sprite sprite, string header, string description)
		{
			image.enabled = true;
			image.sprite = sprite;

			nameText.text = header;
			nameText.color = Color.white;

			descriptionText.text = description;
			
			gradeText.text = "";
		}
	}
}