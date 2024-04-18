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
	public class MDataSO : EditorWindow
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
			{ typeof(SkillData), "SKL" },
			{ typeof(WorldStage), "WS" },
			{ typeof(Dungeon), "D" },
			{ typeof(DungeonStage), "DS" },
			{ typeof(DungeonConstraint), "DC" },
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
			{ typeof(SkillData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(SkillData)}/" },
			{ typeof(WorldStage), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(WorldStage)}/" },
			{ typeof(Dungeon), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Dungeon)}/" },
			{ typeof(DungeonStage), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Dungeon)}/{nameof(DungeonStage)}/" },
			{ typeof(DungeonConstraint), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Dungeon)}/{nameof(DungeonConstraint)}/" },
			{ typeof(Doll), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(Doll)}/" },
			{ typeof(NPC), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(NPC)}/" },
			{ typeof(Monster), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(Monster)}/" },
		};

		public static MDataSO Instance { get; private set; }

		public MDataSODetail MDataSODetail { get; private set; }
		public Dictionary<int, MDataSOSlot> DataSOSlots { get; private set; } = new();
		public MDataSOSlot CurSlot { get; private set; }

		private Dictionary<Type, Dictionary<int, DataSO>> dataSOs;
		private List<DataSO> badIDDataSOs = new();

		private Type CurType { get; set; } = typeof(QuestData);


		[MenuItem("Mascari4615/MDataSO")]
		public static void ShowMDataSO()
		{
			MDataSO wnd = GetWindow<MDataSO>();
			wnd.titleContent = new GUIContent("MDataSO");
		}

		private void OnEnable()
		{
			Debug.Log("OnEnable is executed.");
			Instance = this;

			SOManager soManager = Resources.Load(typeof(SOManager).Name) as SOManager;
			dataSOs = soManager.DataSOs;
			dataSOs.Clear();

			InitList();
			InitDic();
		}

		public void CreateGUI()
		{
			Debug.Log("CreateGUI is executed.");

			VisualElement root = rootVisualElement;
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MDataSO/MDataSO.uxml");

			// Instantiate UXML
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			MDataSODetail = new();

			UpdateGrid();

			Button addButton = rootVisualElement.Q<Button>(name: "BTN_Add");
			addButton.RegisterCallback<ClickEvent>(ev =>
			{
				AddDataSO(MDataSODetail.CurDataSO.GetType());
			});

			VisualElement menu = rootVisualElement.Q<VisualElement>(name: "Menu");
			foreach (Type type in dataSOs.Keys)
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
			// 		MDataSODetail.UpdateCurDataSO(dataDics[CurType].Values.First());
			// 	};
			// };

			SelectDataSOSlot(DataSOSlots.Values.First());

			Selection.selectionChanged += () =>
			{
				// Debug.Log($"Selection.activeObject: {Selection.activeObject}, {Selection.count}"); // "Selection.activeObject: null
				
				if (Selection.activeObject is DataSO dataSO)
				{
					Type type = dataSO.GetType();

					while (type != typeof(DataSO) && dataSOs.ContainsKey(type) == false)
						type = type.BaseType;

					if (type == typeof(DataSO))
						return;

					if (dataSOs[type].ContainsKey(dataSO.ID) == false)
						return;

					SetType(type);
					MDataSOSlot slot = DataSOSlots[dataSO.ID];
					SelectDataSOSlot(slot);
				}
			};
		}

		private void UpdateGrid()
		{
			VisualElement grid = rootVisualElement.Q<VisualElement>(name: "Grid");
			DataSOSlots = new();

			grid.Clear();
			for (int i = 0; i < ID_MAX; i++)
			{
				if (dataSOs[CurType].TryGetValue(i, out DataSO dataSO))
				{
					MDataSOSlot slot = new(dataSO);
					DataSOSlots.Add(i, slot);
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
			badIDDataSOs = new();

			Temp<QuestData>();
			Temp<Card>();
			Temp<Effect>();
			Temp<ItemData>();
			Temp<MonsterWave>();
			Temp<SkillData>();
			Temp<WorldStage>();
			Temp<Dungeon>();
			Temp<DungeonStage>();
			Temp<DungeonConstraint>();
			Temp<Doll>();
			Temp<NPC>();
			Temp<Monster>();

			void Temp<T>() where T : DataSO
			{
				Dictionary<int, DataSO> dic = new();
				InitDic<T>(ref dic, SCRIPTABLE_OBJECTS_DIR, badDataSOList: badIDDataSOs);
				dataSOs.Add(typeof(T), dic);
			}

			void InitDic<T>(ref Dictionary<int, DataSO> dic, string dirPath, bool searchSubDir = true, List<DataSO> badDataSOList = null) where T : DataSO
			{
				const string extension = ".asset";

				DirectoryInfo dir = new(dirPath);
				foreach (FileInfo file in dir.GetFiles())
				{
					if (string.Compare(file.Extension, extension, StringComparison.Ordinal) != 0)
						continue;

					string filePath = $"{dirPath}/{file.Name}";
					Type type = AssetDatabase.GetMainAssetTypeAtPath(filePath);

					if (type == null)
						continue;

					// 만약 type이 T이거나 T의 하위 클래스가 아니면 Continue
					if (type != typeof(T) && !type.IsSubclassOf(typeof(T)))
						continue;

					T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
					if (dic.ContainsKey(asset.ID))
					{
						Debug.LogError($"이미 존재하는 키입니다. {file.Name}");

						if (badDataSOList != null)
							badDataSOList.Add(asset);
					}
					else
					{
						dic.Add(asset.ID, asset);
						
						string goodName = $"{assetPrefixes[typeof(T)]}_{asset.ID}_{asset.Name}";
						// if (asset.name.StartsWith($"{assetPrefixes[typeof(T)]}_{asset.ID}") == false)
						if (asset.name.Equals(goodName) == false)
						{
							Debug.Log($"에셋 이름을 변경합니다. {asset.name} -> {goodName}");
							AssetDatabase.RenameAsset(filePath, goodName);
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

		public DataSO AddDataSO(Type type)
		{
			Dictionary<int, DataSO> dic = dataSOs[type];

			string nName = type.Name;
			// 사용되지 않은 ID를 찾는다.
			int nID = 0;
			while (dic.ContainsKey(nID))
				nID++;

			string assetName = $"{assetPrefixes[type]}_{nID}_{nName}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{assetPaths[type]}{assetName}.asset");

			DataSO newDataSO = CreateInstance(type) as DataSO;
			AssetDatabase.CreateAsset(newDataSO, path);
			newDataSO.ID = nID;
			newDataSO.Name = nName;

			dic.Add(nID, newDataSO);

			UpdateGrid();
			SelectDataSOSlot(DataSOSlots[nID]);
			return newDataSO;
		}

		public DataSO DuplicateDataSO(DataSO dataSO)
		{
			Type type = dataSO.GetType();
			Dictionary<int, DataSO> dic = dataSOs[type];

			string nName = dataSO.Name + " Copy";
			// 사용되지 않은 ID를 찾는다.
			int nID = dataSO.ID + 1;
			while (dic.ContainsKey(nID))
				nID++;

			string assetName = $"{assetPrefixes[type]}_{nID}_{nName}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{assetPaths[type]}{assetName}.asset");

			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(dataSO), path);
			DataSO newDataSO = AssetDatabase.LoadAssetAtPath<DataSO>(path);
			newDataSO.ID = nID;
			newDataSO.Name = nName;

			dic.Add(nID, newDataSO);

			UpdateGrid();
			SelectDataSOSlot(DataSOSlots[nID]);
			return newDataSO;
		}

		public void DeleteDataSO(DataSO dataSO)
		{
			Type type = dataSO.GetType();
			Dictionary<int, DataSO> dic = dataSOs[type];

			int id = dataSO.ID;
			dic.Remove(dataSO.ID);
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(dataSO));

			UpdateGrid();

			SelectDataSOSlot(GetNearSlot(id));
		}

		private MDataSOSlot GetNearSlot(int startID)
		{
			MDataSOSlot slot = null;
			for (int newID = startID; newID < ID_MAX; newID++)
			{
				if (DataSOSlots.TryGetValue(newID, out slot))
					break;
			}
			if (slot == null)
			{
				for (int newID = startID; newID >= 0; newID--)
				{
					if (DataSOSlots.TryGetValue(newID, out slot))
						break;
				}
			}
			return slot;
		}

		public void SelectDataSOSlot(MDataSOSlot slot)
		{
			MDataSOSlot oldSlot = CurSlot;
			CurSlot = slot;
			oldSlot?.UpdateUI();
			CurSlot.UpdateUI();

			MDataSODetail.UpdateCurDataSO(slot.DataSO);
		}
	}
}