using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Mascari4615
{
	// NPC가 다루는 UI 정보
	[Serializable]
	public struct NPCPanelInfo
	{
		public NPCPanelType Type;
		public List<Artifact> Artifacts;
	}

	[CustomPropertyDrawer(typeof(NPCPanelInfo))]
	public class NPCPanelInfo_PropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement container = new();
		
			var popup = new UnityEngine.UIElements.PopupWindow();
			popup.Add(new PropertyField(property.FindPropertyRelative($"{nameof(NPCPanelInfo.Type)}"), "Type"));
			popup.Add(new PropertyField(property.FindPropertyRelative($"{nameof(NPCPanelInfo.Artifacts)}"), "Artifacts"));
			container.Add(popup);
		
			return container;
		}
	}
}