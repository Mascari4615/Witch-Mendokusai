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
		private const string QUEST_DIRECTORY_PATH = "Assets/_Mascari4615/ScriptableObjects/Quest/";
		private const int ID_MAX = 10_000_000;

		// [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
		private List<QuestDataBuffer> questDataBuffers = new();
		private Dictionary<int, QuestData> questDataDic = new();
		private List<QuestData> badIDQuestDatas = new();

		[MenuItem("Mascari4615/MArtifact")]
		public static void ShowMArtifact()
		{
			MArtifact wnd = GetWindow<MArtifact>();
			wnd.titleContent = new GUIContent("MArtifact");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MArtifact/MArtifact.uxml");

			// Instantiate UXML
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			BindAllList();

			System.Diagnostics.Stopwatch sw = new();
			sw.Start();

			VisualElement grid = rootVisualElement.Q<VisualElement>(name: "Grid");
			for (int i = 0; i < ID_MAX; i++)
			{
				if (questDataDic.TryGetValue(i, out QuestData questData))
				{
					MArtifactVisual mAritifactVisual = new(questData);
					mAritifactVisual.RegisterCallback<ClickEvent>(ShowArtifact);
					grid.Add(mAritifactVisual);
				}
			}

			sw.Stop();
			Debug.Log($"TryGetValue x {ID_MAX} = {sw.ElapsedMilliseconds}ms");
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
			InitList();
			InitDic();
		}

		private void OnValidate()
		{
			Debug.Log("OnValidate is executed.");
		}

		private void InitList()
		{
			questDataBuffers = new();
			InitList(ref questDataBuffers, QUEST_DIRECTORY_PATH);

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

					T asset = AssetDatabase.LoadAssetAtPath<T>($"{dirPath}/{file.Name}");
					list.Add(asset);
				}

				if (searchSubDir)
				{
					// dir 아래 모든 폴더 안에 있는 파일을 탐색
					foreach (DirectoryInfo subDir in dir.GetDirectories())
						InitList(ref list, $"{dirPath}/{subDir.Name}/");
				}
			}
		}

		private void InitDic()
		{
			badIDQuestDatas = new();
			questDataDic = new();
			InitDic(ref questDataDic, QUEST_DIRECTORY_PATH, badArtifactList: badIDQuestDatas);

			static void InitDic<T>(ref Dictionary<int, T> dic, string dirPath, bool searchSubDir = true, List<T> badArtifactList = null) where T : Artifact
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

					T asset = AssetDatabase.LoadAssetAtPath<T>($"{dirPath}/{file.Name}");

					if (dic.ContainsKey(asset.ID))
					{
						Debug.LogError($"이미 존재하는 키입니다. {file.Name}");

						if (badArtifactList != null)
							badArtifactList.Add(asset);
					}
					else
					{
						dic.Add(asset.ID, asset);
					}
				}

				if (searchSubDir)
				{
					// dir 아래 모든 폴더 안에 있는 파일을 탐색
					foreach (DirectoryInfo subDir in dir.GetDirectories())
						InitDic(ref dic, $"{dirPath}/{subDir.Name}/");
				}
			}
		}

		private void BindAllList()
		{
			BindEntryList("QuestList", questDataBuffers);

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