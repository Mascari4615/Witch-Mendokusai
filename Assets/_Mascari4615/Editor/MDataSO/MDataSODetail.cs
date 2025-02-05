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
	public class MDataSODetail
	{
		public DataSO CurDataSO { get; private set; }

		private VisualElement root;

		private VisualElement dataSODetail;
		private VisualElement dataSOContent;

		public MDataSODetail()
		{
			Init();
		}

		private void Init()
		{
			Debug.Log(nameof(Init));

			root = MDataSO.Instance.rootVisualElement;

			dataSODetail = root.Q<VisualElement>(name: "DataSODetail");
			dataSOContent = root.Q<VisualElement>(name: "DataSOContent");

			Button duplicateButton = root.Q<Button>(name: "BTN_Dup");
			duplicateButton.clicked += DuplicateCurDataSO;

			Button deleteButton = root.Q<Button>(name: "BTN_Del");
			deleteButton.clicked += DeleteCurDataSO;

			Button changeIDButton = root.Q<Button>(name: "BTN_ChangeID");
			changeIDButton.clicked += () => MDataSO.Instance.IdChanger.SelectDataSO(CurDataSO);

			Debug.Log($"{nameof(Init)} End");
		}

		public void UpdateCurDataSO(DataSO dataSO)
		{
			Debug.Log(nameof(UpdateCurDataSO) + " : " + dataSO.name);

			CurDataSO = dataSO;
			UpdateUI();

			Debug.Log($"{nameof(UpdateCurDataSO)} End");
		}

		public void UpdateUI()
		{
			Debug.Log(nameof(UpdateUI) + " : " + CurDataSO.name);

			SerializedObject serializedObject = new(CurDataSO);

			// CurDataSO의 모든 프로퍼티를 리플렉션으로 가져오기
			List<PropertyInfo> propertyInfos = CurDataSO.GetType()
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
					MDataSO.Instance.DataSOSlots[CurDataSO.ID].UpdateUI();
				});
				dataSOContent.Add(propertyField);

				// 보이지만 수정은 불가능한 프로퍼티
				if (propertyInfo.Name == "ID")
					propertyField.SetEnabled(false);
			}

			Debug.Log($"{nameof(UpdateUI)} End");
		}

		public void DuplicateCurDataSO()
		{
			MDataSO.Instance.DuplicateDataSO(CurDataSO);
		}

		public void DeleteCurDataSO()
		{
			MDataSO.Instance.DeleteDataSO(CurDataSO);
		}
	}
}