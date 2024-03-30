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
	public class MArtifactDetail
	{
		public Artifact CurArtifact { get; private set; }
		private MArtifactSlot curSlot;

		private VisualElement root;

		private VisualElement artifactDetail;
		private VisualElement artifactContent;

		public MArtifactDetail()
		{
			Init();
		}

		private void Init()
		{
			root = MArtifact.Instance.rootVisualElement;

			artifactDetail = root.Q<VisualElement>(name: "ArtifactDetail");
			artifactContent = root.Q<VisualElement>(name: "ArtifactContent");

			Button duplicateButton = root.Q<Button>(name: "BTN_Dup");
			duplicateButton.clicked += DuplicateCurArtifact;

			Button deleteButton = root.Q<Button>(name: "BTN_Del");
			deleteButton.clicked += DeleteCurArtifact;
		}

		public void UpdateCurArtifact(Artifact artifact)
		{
			CurArtifact = artifact;
			UpdateUI();
		}

		public void UpdateUI()
		{
			SerializedObject serializedObject = new(CurArtifact);
		
			// CurArtifact의 모든 프로퍼티를 리플렉션으로 가져오기
			List<PropertyInfo> propertyInfos = CurArtifact.GetType()
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

			// CurArtifact의 모든 프로퍼티를 PropertyBlock으로 만들어서 artifactContent에 추가
			artifactContent.Clear();
			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				if (propertyInfo.Name == "name" || propertyInfo.Name == "hideFlags")
					continue;

				// HACK : 자동으로 생성되는 프로퍼티의 필드의 이름 = <프로퍼티이름>k__BackingField
				PropertyField propertyField = new (serializedObject.FindProperty($"<{propertyInfo.Name}>k__BackingField"));
				propertyField.Bind(serializedObject);
				propertyField.RegisterValueChangeCallback((evt) =>
				{
					serializedObject.ApplyModifiedProperties();
					MArtifact.Instance.MArtifactVisuals[CurArtifact.ID].UpdateUI();
				});
				artifactContent.Add(propertyField);

				// 보이지만 수정은 불가능한 프로퍼티
				if (propertyInfo.Name == "ID")
					propertyField.SetEnabled(false);
			}
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