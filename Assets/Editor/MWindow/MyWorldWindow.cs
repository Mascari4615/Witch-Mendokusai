using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MyWorldWindow : EditorWindow
	{
		private List<FactEntry> factEntries = new();
		private List<RuleEntry> ruleEntries = new();
		private List<EventEntry> eventEntries = new();

		[MenuItem("Mascari4615/MyWorld")]
		static void CreateMenu()
		{
			MyWorldWindow window = GetWindow<MyWorldWindow>();
			window.titleContent = new GUIContent("MyWorld");
		}

		private void OnEnable()
		{
			InitAllEntryList();
		}

		private void InitAllEntryList()
		{
			const string ENTRY_DIRECTORY_PATH = "Assets/_Mascari4615/Entry/";

			InitEntryList(ref factEntries, ENTRY_DIRECTORY_PATH + nameof(FactEntry));
			InitEntryList(ref ruleEntries, ENTRY_DIRECTORY_PATH + nameof(RuleEntry));
			InitEntryList(ref eventEntries, ENTRY_DIRECTORY_PATH + nameof(EventEntry));

			void InitEntryList<T>(ref List<T> list, string dirPath) where T : BaseEntry
			{
				const string extension = ".asset";

				DirectoryInfo dir = new(dirPath);

				// list.Clear();

				// Capacity Reserve
				// 리스트를 새로 만들어 할당하기 때문에 list를 ref으로 받아옴
				list = new List<T>(dir.GetFiles().Length);
				foreach (FileInfo file in dir.GetFiles())
				{
					if (string.Compare(file.Extension, extension, StringComparison.Ordinal) != 0)
						continue;

					// Debug.Log(file.Name);
					list.Add(AssetDatabase.LoadAssetAtPath<T>($"{dirPath}/{file.Name}"));
				}
			}
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MWindow/MWindow.uxml");
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			BindAllEntryList();
		}

		private void BindAllEntryList()
		{
			BindEntryList("FactList", factEntries);
			BindEntryList("RuleList", ruleEntries);
			BindEntryList("EventList", eventEntries);

			void BindEntryList<T>(string listViewName, List<T> list) where T : BaseEntry
			{
				ListView listView = rootVisualElement.Q<ListView>(name: listViewName);

				// Set ListView.itemsSource to populate the data in the list.
				listView.itemsSource = list;

				// Set ListView.makeItem to initialize each entry in the list.
				listView.makeItem = () => new Label();

				// Set ListView.bindItem to bind an initialized entry to a data item.
				listView.bindItem = (VisualElement element, int index) =>
					((Label)element).text = list[index].name;
			}
		}
		private void OnValidate()
		{
			// Debug.Log("OnValidate is executed.");
		}
	}
}