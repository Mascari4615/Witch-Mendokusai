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
	public class MDataSO_IdChanger
	{
		public DataSO CurDataSO { get; private set; }

		private VisualElement root;
		private VisualElement thisRoot;
		private VisualElement body;

		private MDataSOSlot origin;
		private MDataSOSlot target;

		private Button changeButton;

		public MDataSO_IdChanger()
		{
			Init();
		}

		private void Init()
		{
			root = MDataSO.Instance.rootVisualElement;

			thisRoot = root.Q<VisualElement>(name: "IDChanger");
			body = thisRoot.Q<VisualElement>(name: "Body");

			origin = new MDataSOSlot(null);
			target = new MDataSOSlot(null);
			body.Add(origin.VisualElement);
			body.Add(target.VisualElement);

			changeButton = thisRoot.Q<Button>(name: "BTN_ChangeID");
			changeButton.clicked += ChangeID;

			Button closeButton = thisRoot.Q<Button>(name: "BTN_Close");
			closeButton.clicked += Close;

			IntegerField integerField = thisRoot.Q<IntegerField>(name: "IdField");
			integerField.RegisterValueChangedCallback(CheckID);

			UpdateUI();
			MDataSO.Instance.Repaint();
		}

		public void SelectDataSO(DataSO dataSO)
		{
			CurDataSO = dataSO;
			UpdateUI();
		}

		private void UpdateUI()
		{
			Debug.Log("UpdateUI");

			origin.SetDataSO(CurDataSO);
			target.SetDataSO(null);
			
			thisRoot.style.display = CurDataSO == null ? DisplayStyle.None : DisplayStyle.Flex;
		}

		private void CheckID(ChangeEvent<int> evt)
		{
			Debug.Log("CheckID");

			if (CurDataSO == null)
				return;

			int newID = evt.newValue;
			if (newID == CurDataSO.ID)
				return;

			Type type = MDataSO.Instance.GetTypeFromDataSO(CurDataSO);
			if (MDataSO.Instance.DataSOs[type].TryGetValue(newID, out DataSO existingDataSO))
			{
				Debug.Log("ID already exists");
				target.SetDataSO(existingDataSO);
				changeButton.SetEnabled(false);
			}
			else
			{
				target.SetDataSO(null);
				changeButton.SetEnabled(true);
			}
		}

		private void ChangeID()
		{
			Debug.Log(nameof(ChangeID));
		
			if (CurDataSO == null)
				return;

			int newID = thisRoot.Q<IntegerField>(name: "IdField").value;
			if (newID == CurDataSO.ID)
				return;

			Type type = MDataSO.Instance.GetTypeFromDataSO(CurDataSO);
			if (MDataSO.Instance.DataSOs[type].TryGetValue(newID, out DataSO existingDataSO))
			{
				Debug.Log("ID already exists");
				return;
			}

			MDataSO.Instance.DataSOs[type].Remove(CurDataSO.ID);
			CurDataSO.ID = newID;
			MDataSO.Instance.DataSOs[type].Add(newID, CurDataSO);
			MDataSO.Instance.SaveAssets();

			MDataSO.Instance.UpdateGrid();
			MDataSO.Instance.SelectDataSOSlot(MDataSO.Instance.DataSOSlots[CurDataSO.ID]);

			CurDataSO = null;
			UpdateUI();
		}

		private void Close()
		{
			CurDataSO = null;
			UpdateUI();
		}
	}
}