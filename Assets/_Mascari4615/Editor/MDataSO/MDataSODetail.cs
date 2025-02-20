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

			CreateUI();
			UpdateUI();

			return root;
		}

		private void CreateUI()
		{
			// Debug.Log(nameof(CreateUI));

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
			// Debug.Log($"{nameof(MDataSODetail)}.{nameof(CreateUI)} : {dataSO.Name}");

			if (MDataSO.Instance)
			{
				Type baseType = MDataSO.GetBaseType(dataSO);
				if (baseType == typeof(DataSO))
				{
					// Debug.LogWarning("DataSO는 MDataSO UI를 지원하지 않아용");
					return;
				}

				if (MDataSO.Instance.CurType != baseType)
					MDataSO.Instance.SetType(baseType);

				MDataSOSlot dataSOSlot = MDataSO.Instance.GetDataSOSlot(dataSO);
				if (dataSOSlot != null)
					MDataSO.Instance.SelectDataSOSlot(dataSOSlot);
			}

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
						// Debug.Log("Value Changed");
						serializedObject.ApplyModifiedProperties();
						UpdateMDataSOSlot();
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

							// spritePreview.image = previewTexture;

							spritePreviewContainer.Add(spritePreview);
							dataSOContent.Add(spritePreviewContainer);

							// Sprite가 변경되면, Inspector의 SpritePreview도 갱신
							propertyField.RegisterValueChangeCallback((evt) =>
							{
								// Debug.Log("Sprite Changed");

								sprite = (Sprite)propertyInfo.GetValue(dataSO);

								// HACK:
								// 스프라이트 변경 시, 기존 스프라이트와 같은 스프라이트 그룹 내의 스프라이트로 변경하면 프리뷰가 갱신되지 않음 (왜 인지 모르겠음)
								// 그래서 일단 null로 프리뷰 한 번 초기화하고 다시 설정
								// 이러면 동작함
								spritePreview.image = null;

								if (sprite != null)
								{
									Rect uvRect = new Rect(
										sprite.textureRect.x / sprite.texture.width,
										sprite.textureRect.y / sprite.texture.height,
										sprite.textureRect.width / sprite.texture.width,
										sprite.textureRect.height / sprite.texture.height
									);

									spritePreview.image = Sprite.Create(sprite.texture, sprite.textureRect, new Vector2(0.5f, 0.5f)).texture;
									spritePreview.uv = uvRect;
								}
								else
								{
									spritePreview.image = null;
								}
							});
						}
					}

					dataSOContent.Add(propertyField);
				}
			}

			// Debug.Log($"{nameof(UpdateUI)} End");
		}

		private void UpdateMDataSOSlot()
		{
			if (MDataSO.Instance)
			{
				if (MDataSO.Instance.CurType == MDataSO.GetBaseType(dataSO))
				{
					MDataSOSlot dataSOSlot = MDataSO.Instance.GetDataSOSlot(dataSO);
					if (dataSOSlot != null)
						dataSOSlot.UpdateUI();
				}
			}
		}
	}
}