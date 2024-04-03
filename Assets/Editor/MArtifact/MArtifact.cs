using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public class MArtifact : EditorWindow
	{
		public const string SCRIPTABLE_OBJECTS_DIR = "Assets/_Mascari4615/ScriptableObjects/";
		private const int ID_MAX = 10_000_000;

		private readonly Dictionary<Type, string> assetPrefixes = new()
		{
			{ typeof(QuestData), "Q" },
			{ typeof(Card), "C" },
			{ typeof(Effect), "E" },
			{ typeof(ItemData), "I" },
			{ typeof(MonsterWave), "MW" },
			{ typeof(Skill), "SKL" },
			{ typeof(Stage), "STG" },
			{ typeof(Doll), "DOL" },
			{ typeof(NPC), "NPC" },
			{ typeof(Monster), "MOB" },
		};

		private readonly Dictionary<Type, string> assetPaths = new()
		{
			{ typeof(QuestData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(QuestData)}/" },
			{ typeof(Card), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Card)}/" },
			{ typeof(Effect), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Effect)}/" },
			{ typeof(ItemData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(ItemData)}/" },
			{ typeof(MonsterWave), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(MonsterWave)}/" },
			{ typeof(Skill), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Skill)}/" },
			{ typeof(Stage), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Stage)}/" },
			{ typeof(Doll), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(Doll)}/" },
			{ typeof(NPC), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(NPC)}/" },
			{ typeof(Monster), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(Monster)}/" },
		};

		public static MArtifact Instance { get; private set; }

		public MArtifactDetail MArtifactDetail { get; private set; }
		public Dictionary<int, MArtifactSlot> ArtifactSlots { get; private set; } = new();
		public MArtifactSlot CurSlot { get; private set; }

		private readonly Dictionary<Type, Dictionary<int, Artifact>> dataByType = new();
		private List<Artifact> badIDArtifacts = new();

		private Type CurType { get; set; } = typeof(QuestData);

		[MenuItem("Mascari4615/MArtifact")]
		public static void ShowMArtifact()
		{
			MArtifact wnd = GetWindow<MArtifact>();
			wnd.titleContent = new GUIContent("MArtifact");
		}

		private void OnEnable()
		{
			// Debug.Log("OnEnable is executed.");
			Instance = this;

			InitList();
			InitDic();
		}

		public void CreateGUI()
		{
			// Debug.Log("CreateGUI is executed.");
			
			VisualElement root = rootVisualElement;
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MArtifact/MArtifact.uxml");

			// Instantiate UXML
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			MArtifactDetail = new();

			UpdateGrid();

			Button addButton = rootVisualElement.Q<Button>(name: "BTN_Add");
			addButton.RegisterCallback<ClickEvent>(ev =>
			{
				AddArtifact(MArtifactDetail.CurArtifact.GetType());
			});

			VisualElement menu = rootVisualElement.Q<VisualElement>(name: "Menu");
			foreach (Type type in dataByType.Keys)
			{
				Button button = new() { text = type.Name, };
				button.clicked += () => SetType(type);
				menu.Add(button);
			}

			// ListView menu = rootVisualElement.Q<ListView>(name: "MenuList");
			// List<Type> types = dataDics.Keys.ToList();
			// menu.itemsSource = types;
			// menu.makeItem = () => new Button();
			// menu.bindItem = (VisualElement element, int index) =>
			// {
			// 	((Button)element).text = types[index].Name;
			// 	((Button)element).clicked += () =>
			// 	{
			// 		CurType = types[index];
			// 		UpdateGrid();
			// 		MArtifactDetail.UpdateCurArtifact(dataDics[CurType].Values.First());
			// 	};
			// };

			SelectArtifactSlot(ArtifactSlots.Values.First());

			Selection.selectionChanged += () =>
			{
				// Debug.Log($"Selection.activeObject: {Selection.activeObject}, {Selection.count}"); // "Selection.activeObject: null
				
				if (Selection.activeObject is Artifact artifact)
				{
					Type type = artifact.GetType();

					while (type != typeof(Artifact) && dataByType.ContainsKey(type) == false)
						type = type.BaseType;

					if (type == typeof(Artifact))
						return;

					if (dataByType[type].ContainsKey(artifact.ID) == false)
						return;

					SetType(type);
					MArtifactSlot slot = ArtifactSlots[artifact.ID];
					SelectArtifactSlot(slot);
				}
			};
		}

		private void UpdateGrid()
		{
			VisualElement grid = rootVisualElement.Q<VisualElement>(name: "Grid");
			ArtifactSlots = new();

			grid.Clear();
			for (int i = 0; i < ID_MAX; i++)
			{
				if (dataByType[CurType].TryGetValue(i, out Artifact artifact))
				{
					MArtifactSlot slot = new(artifact);
					ArtifactSlots.Add(i, slot);
					grid.Add(slot.VisualElement);
				}
			}

			Repaint();
		}

		private void OnValidate()
		{
			// Debug.Log("OnValidate is executed.");
		}

		private void SetType(Type type)
		{
			CurType = type;
			UpdateGrid();
		}

		private void InitList()
		{
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

			Temp<QuestData>();
			Temp<Card>();
			Temp<Effect>();
			Temp<ItemData>();
			Temp<MonsterWave>();
			Temp<Skill>();
			Temp<Stage>();
			Temp<Doll>();
			Temp<NPC>();
			Temp<Monster>();

			void Temp<T>() where T : Artifact
			{
				Dictionary<int, Artifact> dic = new();
				InitDic<T>(ref dic, SCRIPTABLE_OBJECTS_DIR, badArtifactList: badIDArtifacts);
				dataByType.Add(typeof(T), dic);
			}

			void InitDic<T>(ref Dictionary<int, Artifact> dic, string dirPath, bool searchSubDir = true, List<Artifact> badArtifactList = null) where T : Artifact
			{
				const string extension = ".asset";

				DirectoryInfo dir = new(dirPath);
				foreach (FileInfo file in dir.GetFiles())
				{
					if (string.Compare(file.Extension, extension, StringComparison.Ordinal) != 0)
						continue;

					string filePath = $"{dirPath}/{file.Name}";
					Type type = AssetDatabase.GetMainAssetTypeAtPath(filePath);

					// 만약 type이 T이거나 T의 하위 클래스가 아니면 Continue
					if (type != typeof(T) && !type.IsSubclassOf(typeof(T)))
						continue;

					T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
					if (dic.ContainsKey(asset.ID))
					{
						Debug.LogError($"이미 존재하는 키입니다. {file.Name}");

						if (badArtifactList != null)
							badArtifactList.Add(asset);
					}
					else
					{
						dic.Add(asset.ID, asset);
						
						if (asset.name.StartsWith($"{assetPrefixes[typeof(T)]}_{asset.ID}") == false)
						{
							string newName = $"{assetPrefixes[typeof(T)]}_{asset.ID}_{asset.Name}";
							Debug.Log($"에셋 이름을 변경합니다. {asset.name} -> {newName}");
							AssetDatabase.RenameAsset(filePath, newName);
						}
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

		public Artifact AddArtifact(Type type)
		{
			Dictionary<int, Artifact> dic = dataByType[type];

			string nName = type.Name;
			// 사용되지 않은 ID를 찾는다.
			int nID = 0;
			while (dic.ContainsKey(nID))
				nID++;

			string assetName = $"{assetPrefixes[type]}_{nID}_{nName}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{assetPaths[type]}{assetName}.asset");

			Artifact newArtifact = CreateInstance(type) as Artifact;
			AssetDatabase.CreateAsset(newArtifact, path);
			newArtifact.ID = nID;
			newArtifact.Name = nName;

			dic.Add(nID, newArtifact);

			UpdateGrid();
			SelectArtifactSlot(ArtifactSlots[nID]);
			return newArtifact;
		}

		public Artifact DuplicateArtifact(Artifact artifact)
		{
			Type type = artifact.GetType();
			Dictionary<int, Artifact> dic = dataByType[type];

			string nName = artifact.Name + " Copy";
			// 사용되지 않은 ID를 찾는다.
			int nID = artifact.ID + 1;
			while (dic.ContainsKey(nID))
				nID++;

			string assetName = $"{assetPrefixes[type]}_{nID}_{nName}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{assetPaths[type]}{assetName}.asset");

			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(artifact), path);
			Artifact newArtifact = AssetDatabase.LoadAssetAtPath<Artifact>(path);
			newArtifact.ID = nID;
			newArtifact.Name = nName;

			dic.Add(nID, newArtifact);

			UpdateGrid();
			SelectArtifactSlot(ArtifactSlots[nID]);
			return newArtifact;
		}

		public void DeleteArtifact(Artifact artifact)
		{
			Type type = artifact.GetType();
			Dictionary<int, Artifact> dic = dataByType[type];

			int id = artifact.ID;
			dic.Remove(artifact.ID);
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(artifact));

			UpdateGrid();

			SelectArtifactSlot(GetNearSlot(id));
		}

		private MArtifactSlot GetNearSlot(int startID)
		{
			MArtifactSlot slot = null;
			for (int newID = startID; newID < ID_MAX; newID++)
			{
				if (ArtifactSlots.TryGetValue(newID, out slot))
					break;
			}
			if (slot == null)
			{
				for (int newID = startID; newID >= 0; newID--)
				{
					if (ArtifactSlots.TryGetValue(newID, out slot))
						break;
				}
			}
			return slot;
		}

		public void SelectArtifactSlot(MArtifactSlot slot)
		{
			MArtifactSlot oldSlot = CurSlot;
			CurSlot = slot;
			oldSlot?.UpdateUI();
			CurSlot.UpdateUI();

			MArtifactDetail.UpdateCurArtifact(slot.Artifact);
		}
	}
}