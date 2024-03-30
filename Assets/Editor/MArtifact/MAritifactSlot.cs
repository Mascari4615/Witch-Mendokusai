using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MArtifactSlot : Button
	{
		public Artifact Artifact { get; private set; }

		private readonly Label nameLabel;
		private readonly Label idLabel;

		public MArtifactSlot(Artifact artifact)
		{
			this.Artifact = artifact;

			name = $"{artifact.Name}";
			// style.height
			// style.width
			// style.visibility = Visibility.Hidden;

			nameLabel = new Label() { text = artifact.Name };
			idLabel = new Label() { text = artifact.ID.ToString() };

			Add(nameLabel);
			Add(idLabel);

			// icon.AddToClassList("visual-icon");

			if (artifact.Sprite != null)
				style.backgroundImage = artifact.Sprite.texture;

			RegisterCallback<ClickEvent>(ShowArtifact);
		
			AddToClassList("slot-icons");
		}

		public void UpdateUI()
		{
			nameLabel.text = Artifact.Name;
			idLabel.text = Artifact.ID.ToString();
			if (Artifact.Sprite != null)
				style.backgroundImage = Artifact.Sprite.texture;
		}

		private void ShowArtifact(ClickEvent evt) => UpdateTooltip((evt.currentTarget as MArtifactSlot).Artifact);
		private void ShowArtifact(MouseEnterEvent evt) => UpdateTooltip((evt.currentTarget as MArtifactSlot).Artifact);

		private void UpdateTooltip(Artifact artifact)
		{
			MArtifact.Instance.MArtifactDetail.UpdateCurArtifact(artifact);
		}
	}
}