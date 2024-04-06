using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MDataSOSlot
	{
		public DataSO DataSO { get; private set; }
		public VisualElement VisualElement { get; private set; }

		private readonly Button button;
		private readonly Label nameLabel;
		private readonly Label idLabel;

		public MDataSOSlot(DataSO dataSO)
		{
			VisualTreeAsset treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MDataSO/MDataSOSlot.uxml");
			VisualElement = treeAsset.Instantiate();

			DataSO = dataSO;

			button = VisualElement.Q<Button>();
			nameLabel = VisualElement.Q<Label>(name: "Name");
			idLabel = VisualElement.Q<Label>(name: "ID");

			button.RegisterCallback<ClickEvent>(ShowDataSO);
			UpdateUI();
		}

		public void UpdateUI()
		{
			nameLabel.text = DataSO.Name;
			idLabel.text = DataSO.ID.ToString();
			if (DataSO.Sprite != null)
				button.style.backgroundImage = new(DataSO.Sprite);

			// new Color(226 / 255f, 137 / 255f, 45 / 255f)
			Color borderColor = MDataSO.Instance.CurSlot == this ? Color.white : Color.black;
			button.style.borderTopColor = borderColor;
			button.style.borderBottomColor = borderColor;
			button.style.borderLeftColor =borderColor;
			button.style.borderRightColor = borderColor;
		}

		private void ShowDataSO(ClickEvent evt) => UpdateTooltip();
		private void UpdateTooltip()
		{
			MDataSO.Instance.SelectDataSOSlot(this);
		}
	}
}