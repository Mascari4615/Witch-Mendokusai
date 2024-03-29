using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MArtifactVisual : Button
	{
		public Artifact Artifact { get; private set; }

		public MArtifactVisual(Artifact artifact)
		{
			this.Artifact = artifact;

			name = $"{artifact.Name}";
			// style.height
			// style.width
			// style.visibility = Visibility.Hidden;

			Add(new Label() { text = artifact.Name });
			Add(new Label() { text = artifact.ID.ToString() });

			// icon.AddToClassList("visual-icon");

			if (artifact.Sprite != null)
				style.backgroundImage = artifact.Sprite.texture;

			RegisterCallback<ClickEvent>(ShowArtifact);
		
			AddToClassList("slot-icons");
		}

		private void ShowArtifact(ClickEvent evt) => UpdateTooltip((evt.currentTarget as MArtifactVisual).Artifact);
		private void ShowArtifact(MouseEnterEvent evt) => UpdateTooltip((evt.currentTarget as MArtifactVisual).Artifact);

		private void UpdateTooltip(Artifact artifact)
		{
			MArtifact.Instance.MArtifactDetail.UpdateCurArtifact(artifact);
		}
	}
}