using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	[CustomEditor(typeof(DataSO), true)]
	[CanEditMultipleObjects]
	public class MDataSODetail : Editor
	{
		private DataSO dataSO;
		private VisualElement root;
		private VisualElement dataSOContent;

		public override VisualElement CreateInspectorGUI()
		{
			root = new VisualElement();

			Init();
			UpdateUI();

			return root;
		}

		private void Init()
		{
			// Debug.Log(nameof(Init));

			root.Add(new Label("This is a custom inspector"));

			{
				VisualElement buttonContainer = new VisualElement();
				buttonContainer.style.flexDirection = FlexDirection.Row;
				buttonContainer.style.justifyContent = Justify.SpaceBetween;

				CreateButton("Duplicate", () => MDataSO.Instance.DuplicateDataSO(dataSO));
				CreateButton("Delete", () => MDataSO.Instance.DeleteDataSO(dataSO));
				CreateButton("Select", () => MDataSO.Instance.IdChanger.SelectDataSO(dataSO));

				root.Add(buttonContainer);

				void CreateButton(string text, Action onClick)
				{
					Button button = new Button(onClick) { text = text };
					ApplyButtonStyle(button);
					buttonContainer.Add(button);
				}

				void ApplyButtonStyle(Button button)
				{
					button.style.width = new StyleLength(Length.Percent(30));
					button.style.height = new StyleLength(20);
				}
			}

			root.Add(dataSOContent = new VisualElement());

			dataSO = target as DataSO;
			Debug.Log($"{nameof(MDataSODetail)}.{nameof(Init)} : {dataSO.Name}");
			
			Type baseType = MDataSO.Instance.GetBaseType(dataSO);
			if (MDataSO.Instance.CurType != baseType)
				MDataSO.Instance.SetType(baseType);

			// Debug.Log($"{nameof(Init)} End");
		}

		private void UpdateUI()
		{
			// Debug.Log(nameof(UpdateUI) + " : " + dataSO.name);

			// var defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());
			// root.Add(defaultInspector);

			// CurDataSO의 모든 프로퍼티를 리플렉션으로 가져오기
			{
				List<PropertyInfo> propertyInfos = dataSO.GetType()
				.GetProperties()
				.OrderBy(
					p =>
					{
						var attribute = p.GetCustomAttribute(typeof(PropertyOrderAttribute));
						if (attribute == null)
							return int.MaxValue;
						else
							return ((PropertyOrderAttribute)attribute).Order;
					}).ToList();

				// CurDataSO의 모든 프로퍼티를 PropertyBlock으로 만들어서 dataSOContent에 추가
				dataSOContent.Clear();
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (propertyInfo.Name == "name" || propertyInfo.Name == "hideFlags")
						continue;

					// HACK : 자동으로 생성되는 프로퍼티의 필드의 이름 = <프로퍼티이름>k__BackingField
					PropertyField propertyField = new(serializedObject.FindProperty($"<{propertyInfo.Name}>k__BackingField"));
					propertyField.Bind(serializedObject);

					propertyField.RegisterValueChangeCallback((evt) =>
					{
						serializedObject.ApplyModifiedProperties();

						Type baseType = MDataSO.Instance.GetBaseType(dataSO);
						if (MDataSO.Instance.CurType != baseType)
							MDataSO.Instance.SetType(baseType);

						// Debug.Log(MDataSO.Instance.DataSOSlots.Count);
						MDataSO.Instance.DataSOSlots[dataSO.ID].UpdateUI();
					});

					// 보이지만 수정은 불가능한 프로퍼티
					if (propertyInfo.Name == "ID")
						propertyField.SetEnabled(false);

					if (propertyInfo.PropertyType == typeof(Sprite))
					{
						VisualElement spritePreviewContainer = new VisualElement();
						spritePreviewContainer.style.flexDirection = FlexDirection.RowReverse;

						Sprite sprite = (Sprite)propertyInfo.GetValue(dataSO);

						if (sprite == null)
						{
							Label noSpriteLabel = new Label("No Sprite");
							noSpriteLabel.style.marginLeft = 10;
							noSpriteLabel.style.marginTop = 10;
							spritePreviewContainer.Add(noSpriteLabel);
							dataSOContent.Add(spritePreviewContainer);
						}
						else
						{
							Image spritePreview = new Image
							{
								scaleMode = ScaleMode.ScaleToFit,
								style =
								{
									width = 64,
									height = 64,
									marginLeft = 10,
									marginTop = 10
								}
							};

							// Sprite의 UV 설정
							Rect uvRect = new Rect(
								sprite.textureRect.x / sprite.texture.width,
								sprite.textureRect.y / sprite.texture.height,
								sprite.textureRect.width / sprite.texture.width,
								sprite.textureRect.height / sprite.texture.height
							);

							// Sprite의 특정 영역을 잘라내어 표시
							spritePreview.image = Sprite.Create(sprite.texture, sprite.textureRect, new Vector2(0.5f, 0.5f)).texture;
							spritePreview.uv = uvRect;

							spritePreviewContainer.Add(spritePreview);
							dataSOContent.Add(spritePreviewContainer);

							// Sprite가 변경될 때 프리뷰 업데이트
							propertyField.RegisterValueChangeCallback((evt) =>
							{
								sprite = (Sprite)propertyInfo.GetValue(dataSO);
								if (sprite != null)
								{
									uvRect = new Rect(
										sprite.textureRect.x / sprite.texture.width,
										sprite.textureRect.y / sprite.texture.height,
										sprite.textureRect.width / sprite.texture.width,
										sprite.textureRect.height / sprite.texture.height
									);

									spritePreview.image = Sprite.Create(sprite.texture, sprite.textureRect, new Vector2(0.5f, 0.5f)).texture;
									spritePreview.uv = uvRect;
								}
							});
						}
					}

					dataSOContent.Add(propertyField);
				}
			}

			// Debug.Log($"{nameof(UpdateUI)} End");
		}
	}
}