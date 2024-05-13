using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mascari4615
{
	public partial class MDataSO : EditorWindow
	{
		public const string SCRIPTABLE_OBJECTS_DIR = "Assets/_Mascari4615/ScriptableObjects/";
		private const int ID_MAX = 100_000_000;

		private readonly Dictionary<Type, string> assetPrefixes = new()
		{
			{ typeof(QuestSO), "Q" },
			{ typeof(CardData), "C" },
			{ typeof(ItemData), "I" },
			{ typeof(MonsterWave), "MW" },
			{ typeof(ObjectData), "O"},
			{ typeof(SkillData), "SKL" },
			{ typeof(StatData), "ST"},
			{ typeof(StatisticsData), "STS"},
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
			{ typeof(QuestSO), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(QuestSO)}/" },
			{ typeof(CardData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(CardData)}/" },
			{ typeof(ItemData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(ItemData)}/" },
			{ typeof(MonsterWave), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(MonsterWave)}/" },
			{ typeof(ObjectData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(ObjectData)}/" },
			{ typeof(SkillData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(SkillData)}/" },
			{ typeof(StatData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(StatData)}/"},
			{ typeof(StatisticsData), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(StatisticsData)}/"},
			{ typeof(WorldStage), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(WorldStage)}/" },
			{ typeof(Dungeon), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Dungeon)}/" },
			{ typeof(DungeonStage), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Dungeon)}/{nameof(DungeonStage)}/" },
			{ typeof(DungeonConstraint), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Dungeon)}/{nameof(DungeonConstraint)}/" },
			{ typeof(Doll), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(Doll)}/" },
			{ typeof(NPC), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(NPC)}/" },
			{ typeof(Monster), $"{SCRIPTABLE_OBJECTS_DIR}{nameof(Unit)}/{nameof(Monster)}/" },
		};

		public static MDataSO Instance { get; private set; }

		public MDataSODetail Detail { get; private set; }
		public MDataSO_IdChanger IdChanger { get; private set; }
		public Dictionary<int, MDataSOSlot> DataSOSlots { get; private set; } = new();
		public MDataSOSlot CurSlot { get; private set; }

		public Dictionary<Type, Dictionary<int, DataSO>> DataSOs { get; private set; }
		public Dictionary<int, List<DataSO>> BadIDDataSOs { get; private set; } = new();

		public Type CurType { get; private set; } = typeof(QuestSO);

		private bool isInit = false;


		[MenuItem("Mascari4615/MDataSO")]
		public static void ShowMDataSO()
		{
			MDataSO wnd = GetWindow<MDataSO>();
			wnd.titleContent = new GUIContent("MDataSO");
		}

		private void OnEnable()
		{
			Debug.Log(nameof(OnEnable));

			Instance = this;
			DataSOs = new();

			InitList();

			InitEnumData<StatData, StatType>();
			InitEnumData<StatisticsData, StatisticsType>();

			SaveAssets();
		}

		public void CreateGUI()
		{
			Debug.Log(nameof(CreateGUI));

			VisualElement root = rootVisualElement;
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MDataSO/MDataSO.uxml");

			// Instantiate UXML
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			Detail = new();
			IdChanger = new();

			UpdateGrid();

			Button addButton = rootVisualElement.Q<Button>(name: "BTN_Add");
			addButton.RegisterCallback<ClickEvent>(ev =>
			{
				AddDataSO(Detail.CurDataSO.GetType());
			});

			DropdownField dropdown = rootVisualElement.Q<DropdownField>(name: "Menu");
			dropdown.choices = assetPaths.Keys.Select(type => type.Name).ToList();
			dropdown.RegisterValueChangedCallback(ev =>
			{
				string typeName = ev.newValue;
				Type type = assetPaths.Keys.First(t => t.Name == typeName);
				SetType(type);
			});

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

					while (type != typeof(DataSO) && assetPaths.ContainsKey(type) == false)
						type = type.BaseType;

					if (type == typeof(DataSO))
						return;

					SetType(type);
					MDataSOSlot slot = DataSOSlots[dataSO.ID];
					SelectDataSOSlot(slot);
				}
			};

			isInit = true;
			Debug.Log("OnEnable is executed.");
		}

		public void UpdateGrid()
		{
			Debug.Log(nameof(UpdateGrid));

			VisualElement grid = rootVisualElement.Q<VisualElement>(name: "Grid");
			grid.Clear();

			InitDic(CurType);
			Dictionary<int, DataSO> dataSOs = DataSOs[CurType];

			DataSOSlots.Clear();
			for (int i = 0; i < ID_MAX; i++)
			{
				if (dataSOs.TryGetValue(i, out DataSO dataSO))
				{
					MDataSOSlot slot = new((slot) => SelectDataSOSlot(slot));
					slot.SetDataSO(dataSO);
					DataSOSlots.Add(i, slot);
					grid.Add(slot.VisualElement);
				}
			}

			SelectDataSOSlot(DataSOSlots.Values.First());
			Repaint();
		}

		private void OnValidate()
		{
			Debug.Log("OnValidate is executed.");
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

		private void InitDic(Type type)
		{
			Debug.Log($"{nameof(InitDic)} <{type.Name}>");

			Dictionary<int, DataSO> dic = DataSOs[type] = new();
			InitDic(ref dic, type, SCRIPTABLE_OBJECTS_DIR);
			SaveAssets();

			if (BadIDDataSOs.Count > 0)
			{
				if (isInit)
				{
					Debug.Log(IdChanger);
					IdChanger.StartProcessBadIdDataSOs();
				}
			}

			void InitDic(ref Dictionary<int, DataSO> dic, Type type, string dirPath, bool searchSubDir = true)
			{
				const string extension = ".asset";

				DirectoryInfo dir = new(dirPath);
				foreach (FileInfo file in dir.GetFiles())
				{
					if (string.Compare(file.Extension, extension, StringComparison.Ordinal) != 0)
						continue;

					string filePath = $"{dirPath}/{file.Name}";
					Type fileType = AssetDatabase.GetMainAssetTypeAtPath(filePath);

					if (fileType == null)
						continue;

					// 만약 type이 T이거나 T의 하위 클래스가 아니면 Continue
					if (fileType != type && !fileType.IsSubclassOf(type))
						continue;

					DataSO asset = AssetDatabase.LoadAssetAtPath<DataSO>(filePath);
					if (dic.ContainsKey(asset.ID))
					{
						Debug.LogWarning($"이미 존재하는 키입니다. {file.Name}");

						if (BadIDDataSOs.ContainsKey(asset.ID) == false)
						{
							BadIDDataSOs.Add(asset.ID, new());
							if (BadIDDataSOs[asset.ID].Contains(dic[asset.ID]) == false)
								BadIDDataSOs[asset.ID].Add(dic[asset.ID]);
						}
						
						if (BadIDDataSOs[asset.ID].Contains(asset) == false)
							BadIDDataSOs[asset.ID].Add(asset);
					}
					else
					{
						dic.Add(asset.ID, asset);

						string goodName = $"{assetPrefixes[type]}_{asset.ID}_{asset.Name}";
						// if (asset.name.StartsWith($"{assetPrefixes[type]}_{asset.ID}") == false)

						// 파일 이름에 사용할 수 없는 문자를 제거
						Regex regex = new(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))));
						goodName = regex.Replace(goodName, string.Empty);

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
						InitDic(ref dic, type, $"{dirPath}/{subDir.Name}/");
				}
			}
		}

		public DataSO AddDataSO(Type type, int nID = -1, string nName = null)
		{
			Debug.Log(nameof(AddDataSO));

			Dictionary<int, DataSO> dic = DataSOs[type];

			// 사용되지 않은 ID를 찾는다.
			if (nID == -1)
			{
				nID = 0;
				while (dic.ContainsKey(nID))
					nID++;
			}

			if (nName == null)
				nName = $"New_{type.Name}";

			string assetName = $"{assetPrefixes[type]}_{nID}_{nName}";
			string path = AssetDatabase.GenerateUniqueAssetPath($"{assetPaths[type]}{assetName}.asset");

			Debug.Log($"AddDataSO: {type.Name} {nID} {nName} {path}");

			DataSO newDataSO = CreateInstance(type) as DataSO;
			AssetDatabase.CreateAsset(newDataSO, path);
			newDataSO.ID = nID;
			newDataSO.Name = nName;

			EditorUtility.SetDirty(newDataSO);
			AssetDatabase.SaveAssets();

			dic.Add(nID, newDataSO);

			if (isInit)
			{
				UpdateGrid();
				SelectDataSOSlot(DataSOSlots[nID]);
			}

			return newDataSO;
		}

		public DataSO DuplicateDataSO(DataSO dataSO)
		{
			Debug.Log(nameof(DuplicateDataSO));

			Type type = GetTypeFromDataSO(dataSO);

			if (type == typeof(DataSO) || DataSOs[type].ContainsKey(dataSO.ID) == false)
			{
				Debug.LogError("복사할 수 없는 데이터입니다.");
				return null;
			}

			Dictionary<int, DataSO> dic = DataSOs[type];

			string nName = dataSO.Name;
			
			// 기존 데이터가 숫자로 끝나면, 해당 숫자에 1을 더한 값을 붙인다.
			Match match = Regex.Match(nName, @"\d+$");
			if (match.Success)
			{
				string number = match.Value;
				nName = nName.Substring(0, nName.Length - number.Length) + (int.Parse(number) + 1);
			}
			// 아니라면 "_Copy"를 붙인다.
			else
			{
				nName += "_Copy";
			}

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

		public Type GetTypeFromDataSO(DataSO dataSO)
		{
			Type type = dataSO.GetType();
			while (type != typeof(DataSO) && DataSOs.ContainsKey(type) == false)
				type = type.BaseType;

			return type;
		}

		public void DeleteDataSO(DataSO dataSO)
		{
			Debug.Log(nameof(DeleteDataSO));

			Type type = GetTypeFromDataSO(dataSO);

			if (type == typeof(DataSO) || DataSOs[type].ContainsKey(dataSO.ID) == false)
			{
				Debug.LogError("삭제할 수 없는 데이터입니다.");
				return;
			}

			Dictionary<int, DataSO> dic = DataSOs[type];

			int id = dataSO.ID;

			if (dic.ContainsKey(id))
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
			Type type = GetTypeFromDataSO(slot.DataSO);

			if (CurType != type)
				SetType(type);

			MDataSOSlot oldSlot = CurSlot;
			CurSlot = slot;
			oldSlot?.UpdateUI();
			CurSlot.UpdateUI();

			Detail.UpdateCurDataSO(slot.DataSO);
		}

		public void InitEnumData<TData, TEnum>() where TData : DataSO
		{
			Debug.Log($"{nameof(InitEnumData)} <{typeof(TData).Name}, {typeof(TEnum).Name}>");

			InitDic(typeof(TData));

			const string PropertyName = "Type";

			var dic = DataSOs[typeof(TData)];
			foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
			{
				if (dic.TryGetValue(Convert.ToInt32(enumValue), out DataSO dataSO))
				{
					TData typedData = dataSO as TData;

					string goodName = Enum.GetName(typeof(TEnum), enumValue);
					if (typedData.Name != goodName)
					{
						Debug.Log($"{typedData.name}의 이름을 업데이트합니다. {typedData.Name} -> {goodName}");
						typedData.Name = goodName;
						EditorUtility.SetDirty(typedData);
					}

					PropertyInfo typeProperty = typeof(TData).GetProperty(PropertyName);
					if (enumValue.ToString() != typeProperty.GetValue(typedData).ToString())
					{
						Debug.Log($"{typedData.name}의 Type을 업데이트합니다. {typeProperty.GetValue(typedData)} -> {enumValue}");
						typeProperty.SetValue(typedData, (int)Enum.Parse(typeof(TEnum), enumValue.ToString()));
						EditorUtility.SetDirty(typedData);
					}
				}
				else
				{
					Debug.Log($"Data를 추가합니다.");
					Type type = typeof(TData);
					int nID = Convert.ToInt32(enumValue);
					string nName = Enum.GetName(typeof(TEnum), enumValue);

					TData typedData = AddDataSO(type, nID, nName) as TData;
					PropertyInfo typeProperty = typeof(TData).GetProperty(PropertyName);
					typeProperty.SetValue(typedData, nID);
				}
			}
		}

		public void SaveAssets()
		{
			Debug.Log(nameof(SaveAssets));

			foreach (var dic in DataSOs.Values)
				foreach (DataSO dataSO in dic.Values)
					EditorUtility.SetDirty(dataSO);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log($"{nameof(SaveAssets)} is executed.");
		}
	}
}