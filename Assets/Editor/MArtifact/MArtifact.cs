using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MArtifact : EditorWindow
	{
		// [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
		private List<QuestData> questDatas = new();

		[MenuItem("Mascari4615/MArtifact")]
		public static void ShowExample()
		{
			MArtifact wnd = GetWindow<MArtifact>();
			wnd.titleContent = new GUIContent("MArtifact");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// VisualElements objects can contain other VisualElement following a tree hierarchy.
			// VisualElement label = new Label("Hello World! From C#");
			// root.Add(label);

			// Button button = new Button();
			// button.name = "button3";
			// button.text = "This is button3.";
			// root.Add(button);

			// Toggle toggle = new Toggle();
			// toggle.name = "toggle3";
			// toggle.label = "Number?";
			// root.Add(toggle);

			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MArtifact/MArtifact.uxml");

			// Instantiate UXML
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			BindAllList();

			VisualElement grid = rootVisualElement.Q<VisualElement>(name: "Grid");
			foreach (QuestData questData in questDatas)
			{
				MArtifactVisual mAritifactVisual = new(questData);
				mAritifactVisual.RegisterCallback<ClickEvent>(ShowArtifact);

				// 마우스를 올렸을때
				// mAritifactVisual.RegisterCallback<MouseEnterEvent>(ShowArtifact);

				grid.Add(mAritifactVisual);
			}

			// SlotIcon.style.backgroundImage = questDatas[0].Sprite.texture;

			//Call the event handler
			// SetupButtonHandler();
		}

		private void ShowArtifact(ClickEvent evt) => UpdateTooltip((evt.currentTarget as MArtifactVisual).Artifact);
		private void ShowArtifact(MouseEnterEvent evt) => UpdateTooltip((evt.currentTarget as MArtifactVisual).Artifact);

		private void UpdateTooltip(Artifact artifact)
		{
			VisualElement root = rootVisualElement;

			Label nameLabel = root.Q<Label>(name: nameof(Artifact.Name));
			nameLabel.text = artifact.Name;

			Label descriptionLabel = root.Q<Label>(name: nameof(Artifact.Description));
			descriptionLabel.text = artifact.Description;
		}

		private void OnEnable()
		{
			Debug.Log("OnEnable is executed.");
			InitAllList();
		}

		private void OnValidate()
		{
			Debug.Log("OnValidate is executed.");
		}

		private void InitAllList()
		{
			const string QUEST_DIRECTORY_PATH = "Assets/_Mascari4615/ScriptableObjects/Quest/";
			questDatas = new();
			InitList(ref questDatas, QUEST_DIRECTORY_PATH);

			static void InitList<T>(ref List<T> list, string dirPath, bool searchSubDir = true) where T : ScriptableObject
			{
				const string extension = ".asset";

				DirectoryInfo dir = new(dirPath);
				foreach (FileInfo file in dir.GetFiles())
				{
					if (string.Compare(file.Extension, extension, StringComparison.Ordinal) != 0)
						continue;

					// QuestData 스크립터블 객체가 아니면 Continue
					if (AssetDatabase.GetMainAssetTypeAtPath($"{dirPath}/{file.Name}") != typeof(T))
						continue;

					// Debug.Log(file.Name);
					list.Add(AssetDatabase.LoadAssetAtPath<T>($"{dirPath}/{file.Name}"));
				}

				if (searchSubDir)
				{
					// dir 아래 모든 폴더 안에 있는 파일을 탐색
					foreach (DirectoryInfo subDir in dir.GetDirectories())
						InitList(ref list, $"{dirPath}/{subDir.Name}/");
				}
			}
		}

		private void BindAllList()
		{
			BindEntryList("QuestList", questDatas);

			// 설명: 리스트뷰에 데이터를 바인딩하는 함수
			// 매개변수: 리스트뷰 이름, 바인딩할 리스트
			// 제네릭: T는 BaseEntry를 상속받은 클래스여야 함
			void BindEntryList<T>(string listViewName, List<T> list) where T : ScriptableObject
			{
				// Q(Query) 함수는 VisualElement의 자식 요소를 찾는 함수
				// 즉 rootVisualElement의 자식 요소 중에서 이름이 listViewName인 요소를 찾아서 반환
				ListView listView = rootVisualElement.Q<ListView>(name: listViewName);

				// Set ListView.itemsSource to populate the data in the list.
				// 리스트뷰에 데이터를 채우기 위해 ListView.itemsSource를 설정합니다.
				// itemSource는 리스트뷰에 표시할 데이터를 설정하는 속성입니다.
				// 리스트뷰에 표시할 데이터를 설정하면 리스트뷰에 데이터가 표시됩니다.
				listView.itemsSource = list;

				// Set ListView.makeItem to initialize each entry in the list.
				// ListView.makeItem을 설정하여 목록의 각 항목을 초기화합니다.
				// makeItem은 리스트뷰에 표시할 각 항목을 초기화하는 함수입니다.
				// makeItem은 항목을 초기화하는 데 사용할 VisualElement를 반환합니다.
				listView.makeItem = () => new Label();

				// 설명 : ListView.bindItem은 ListView.makeItem에서 반환한 VisualElement와 데이터를 바인딩하는 함수입니다.
				listView.bindItem = (VisualElement element, int index) =>
					((Label)element).text = list[index].name;
			}
		}
	}
}