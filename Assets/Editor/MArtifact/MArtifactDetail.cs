using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MArtifactDetail
	{
		public Artifact CurArtifact { get; private set; }

		private readonly VisualElement root;
		private readonly Label nameLabel;
		private readonly Label descriptionLabel;
		private readonly Button duplicateButton;
		private readonly Button deleteButton;

		public MArtifactDetail()
		{
			VisualElement root = MArtifact.Instance.rootVisualElement;

			nameLabel = root.Q<Label>(name: nameof(Artifact.Name));
			descriptionLabel = root.Q<Label>(name: nameof(Artifact.Description));

			duplicateButton = root.Q<Button>(name: "BTN_Dup");
			duplicateButton.clicked += DuplicateCurArtifact;

			deleteButton = root.Q<Button>(name: "BTN_Del");
			deleteButton.clicked += DeleteCurArtifact;
		}

		public void UpdateCurArtifact(Artifact artifact)
		{
			CurArtifact = artifact;
			UpdateUI();
		}

		public void UpdateUI()
		{
			nameLabel.text = CurArtifact.Name;
			descriptionLabel.text = CurArtifact.Description;
		}

		public void DuplicateCurArtifact()
		{
			MArtifact.Instance.DuplicateArtifact(CurArtifact);
		}

		public void DeleteCurArtifact()
		{
			MArtifact.Instance.DeleteArtifact(CurArtifact);
		}
	}
}