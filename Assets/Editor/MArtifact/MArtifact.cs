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
		// MArtifact window = EditorWindow.GetWindow<MArtifact>()
		public static MArtifact Instance { get; private set; }

		public MArtifactDetail MArtifactDetail { get; private set; }

		public const string QUEST_DIRECTORY_PATH = "Assets/_Mascari4615/ScriptableObjects/Quest/";
		private const int ID_MAX = 10_000_000;

		// [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
		private List<QuestDataBuffer> questDataBuffers = new();
		private readonly Dictionary<Type, Dictionary<int, Artifact>> dataDics = new();
		private List<QuestData> badIDArtifacts = new();

		[MenuItem("Mascari4615/MArtifact")]
		public static void ShowMArtifact()
		{
			MArtifact wnd = GetWindow<MArtifact>();
			wnd.titleContent = new GUIContent("MArtifact");
		}

		public void CreateGUI()
		{
			Debug.Log("CreateGUI is executed.");
			
			VisualElement root = rootVisualElement;
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MArtifact/MArtifact.uxml");

			// Instantiate UXML
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			MArtifactDetail = new();

			BindAllList();
			UpdateGrid();
		}

		private void UpdateGrid()
		{
			VisualElement grid = rootVisualElement.Q<VisualElement>(name: "Grid");
			
			grid.Clear();
			Dictionary<int, Artifact> targetDic = dataDics[typeof(QuestData)];
			for (int i = 0; i < ID_MAX; i++)
			{
				if (targetDic.TryGetValue(i, out Artifact artifact))
					grid.Add(new MArtifactVisual(artifact));
			}
			Repaint();
		}

		private void OnEnable()
		{
			Debug.Log("OnEnable is executed.");
			Instance = this;

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
			badIDArtifacts = new();
			Dictionary<int, Artifact> questDataDic = new();
			InitDic<QuestData>(ref questDataDic, QUEST_DIRECTORY_PATH, badArtifactList: badIDArtifacts);
			dataDics.Add(typeof(QuestData), questDataDic);

			static void InitDic<T>(ref Dictionary<int, Artifact> dic, string dirPath, bool searchSubDir = true, List<T> badArtifactList = null) where T : Artifact
			{
				const string extension = ".asset";

				DirectoryInfo dir = new(dirPath);
				foreach (FileInfo file in dir.GetFiles())
				{
					if (string.Compare(file.Extension, extension, StringComparison.Ordinal) != 0)
						continue;

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
						InitDic<T>(ref dic, $"{dirPath}/{subDir.Name}/");
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

		public void DuplicateArtifact(Artifact artifact)
		{
			Type type = artifact.GetType();
			Dictionary<int, Artifact> dic = dataDics[type];

			string nName = artifact.Name + " Copy";
			// 사용되지 않은 ID를 찾는다.
			int nID = artifact.ID + 1;
			while (dic.ContainsKey(nID))
				nID++;

			string assetName = $"Q_{nID}_{nName}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{QUEST_DIRECTORY_PATH}{assetName}.asset");

			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(artifact), path);
			Artifact newArtifact = AssetDatabase.LoadAssetAtPath<Artifact>(path);
			newArtifact.ID = nID;
			newArtifact.Name = nName;

			dic.Add(nID, newArtifact);

			UpdateGrid();
		}

		public void DeleteArtifact(Artifact artifact)
		{
			Type type = artifact.GetType();
			Dictionary<int, Artifact> dic = dataDics[type];

			string assetName = $"Q_{artifact.ID}_{artifact.Name}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{QUEST_DIRECTORY_PATH}{assetName}.asset");

			dic.Remove(artifact.ID);
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(artifact));

			UpdateGrid();
		}
	}
}