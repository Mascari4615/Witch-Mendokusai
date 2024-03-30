using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MArtifactSlot
	{
		public Artifact Artifact { get; private set; }
		public VisualElement VisualElement { get; private set; }

		private readonly Button button;
		private readonly Label nameLabel;
		private readonly Label idLabel;

		public MArtifactSlot(Artifact artifact)
		{
			VisualTreeAsset treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MArtifact/MArtifactSlot.uxml");
			VisualElement = treeAsset.Instantiate();

			Artifact = artifact;

			button = VisualElement.Q<Button>();
			nameLabel = VisualElement.Q<Label>(name: "Name");
			idLabel = VisualElement.Q<Label>(name: "ID");

			button.RegisterCallback<ClickEvent>(ShowArtifact);
			UpdateUI();
		}

		public void UpdateUI()
		{
			nameLabel.text = Artifact.Name;
			idLabel.text = Artifact.ID.ToString();
			if (Artifact.Sprite != null)
				button.style.backgroundImage = new(Artifact.Sprite);

			// new Color(226 / 255f, 137 / 255f, 45 / 255f)
			Color borderColor = MArtifact.Instance.CurSlot == this ? Color.white : Color.black;
			button.style.borderTopColor = borderColor;
			button.style.borderBottomColor = borderColor;
			button.style.borderLeftColor =borderColor;
			button.style.borderRightColor = borderColor;
		}

		private void ShowArtifact(ClickEvent evt) => UpdateTooltip();
		private void UpdateTooltip()
		{
			MArtifact.Instance.SelectArifactSlot(this);
		}
	}
}