using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;
using Newtonsoft.Json;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class DataManager : Singleton<DataManager>
	{
		public WorkManager WorkManager { get; private set; }
		public QuestManager QuestManager { get; private set; }
		public readonly Dictionary<string, int> CraftDic = new();

		private SOManager SOManager;

		public bool IsInited { get; private set; }
		public int CurDollID;
		public int DummyDollCount;

		public string localDisplayName = "";

		public PlayFabManager PlayFabManager { get; private set; }

		// HACK
		[field: SerializeField] public bool UseLocalData { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			SOManager = SOManager.Instance;
			Debug.Log($"{SOManager.DataSOs}");
			Debug.Log($"{SOManager.DataSOs.Count}");

			foreach (KeyValuePair<Type, Dictionary<int, DataSO>> dataSO in SOManager.DataSOs)
			{
				Debug.Log($"{dataSO.Key}");
				Debug.Log($"{dataSO.Value}");
				Debug.Log($"{dataSO.Value.Count}");
			}

			PlayFabManager = GetComponent<PlayFabManager>();
			WorkManager = new();
			QuestManager = new();

			ForEach<ItemData>(itemData => 
			{
				if (itemData.Recipes == null)
					return;

				foreach (Recipe recipe in itemData.Recipes)
				{
					List<int> recipeToList = recipe.Ingredients.Select(ingredient => ingredient.ID).ToList();
					recipeToList.Sort();
					CraftDic.Add(string.Join(',', recipeToList), itemData.ID);
				}
			});

			if (UseLocalData)
			{
				string path = Path.Combine(Application.dataPath, "WM.json");

				if (File.Exists(path))
				{
					string json = File.ReadAllText(path);
					LoadData(JsonConvert.DeserializeObject<GameData>(json, new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.Auto
					}));
				}
				else
				{
					CreateNewGameData();
				}
			}
		}

		public void CreateNewGameData()
		{
			GameData newGameData = new()
			{
				curDollIndex = 0,
				dummyDollCount = 1,
				nyang = 100,
				itemInventoryItems = new(),
				dollDataList = new()
				{
					new(0, 1, 0, new(){})
				},
				works = new()
				{
					{ WorkListType.DollWork, new() },
					{ WorkListType.DummyWork, new() },
					{ WorkListType.VQuestWork, new() }
				},
				questDataList = new(),
				quests = new(),
				statistics = new()
			};

			// 아이템(장비) 초기화
			Doll defaultDoll = GetDoll(0);
			defaultDoll.EquipmentGuids.Clear();

			Inventory inventory = SOManager.ItemInventory;
			inventory.Load(newGameData.itemInventoryItems);
			foreach (EquipmentData equipmentData in defaultDoll.DefaultEquipments)
			{
				inventory.Add(equipmentData);
				Guid? guid = inventory.GetItem(inventory.FindItemIndex(equipmentData)).Guid;
				newGameData.dollDataList[0].EquipmentGuids.Add(guid);
				defaultDoll.EquipmentGuids.Add(guid);
			}
			defaultDoll.EquipmentGuids.Add(null);
			newGameData.itemInventoryItems = inventory.Save();

			// 장비 초기화 이후 저장
			ForEach<Doll>(doll =>
			{
				if (doll.ID != 0)
					newGameData.dollDataList.Add(doll.Save());
			});
			ForEach<QuestData>(questData => newGameData.questDataList.Add(questData.Save()));

			SaveData();
			LoadData(newGameData);
		}

		public void LoadData(GameData saveData)
		{
			CurDollID = saveData.curDollIndex;
			DummyDollCount = saveData.dummyDollCount;
			SOManager.Nyang.RuntimeValue = saveData.nyang;

			// 통계 초기화
			SOManager.Statistics.Load(saveData.statistics);

			// 아이템 초기화
			SOManager.ItemInventory.Load(saveData.itemInventoryItems);

			// 인형 초기화
			SOManager.DollBuffer.Clear();
			foreach (DollData dollData in saveData.dollDataList)
			{
				GetDoll(dollData.DollID).Load(dollData);
				SOManager.DollBuffer.Add(GetDoll(dollData.DollID));
			}
			for (int i = 0; i < saveData.dummyDollCount - 1; i++)
				SOManager.DollBuffer.Add(GetDoll(Doll.DUMMY_ID));

			// 퀘스트 초기화
			foreach (QuestDataSave questData in saveData.questDataList)
			{
				GetQuest(questData.QuestID).Load(questData);
				if (questData.State >= QuestDataState.Unlocked)
					SOManager.QuestDataBuffer.Add(GetQuest(questData.QuestID));
			}

			// 작업 초기화
			WorkManager.Init(saveData.works);
			QuestManager.Init(saveData.quests);

			IsInited = true;
		}

		public void SaveData()
		{
			GameData gameData = new()
			{
				curDollIndex = CurDollID,
				dummyDollCount = DummyDollCount,
				nyang = SOManager.Nyang.RuntimeValue,
				itemInventoryItems = SOManager.ItemInventory.Save(),
				dollDataList = new(),
				works = WorkManager.Works,
				questDataList = new(),
				quests = QuestManager.Quests.Datas,
				statistics = SOManager.Statistics.Save()
			};

			ForEach<Doll>(doll => gameData.dollDataList.Add(doll.Save()));
			ForEach<QuestData>(questData => gameData.questDataList.Add(questData.Save()));

			if (UseLocalData)
			{
				string json = JsonConvert.SerializeObject(gameData, Formatting.Indented, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});
				string path = Path.Combine(Application.dataPath, "WM.json");
				File.WriteAllText(path, json);
			}
			else
			{
				PlayFabManager.SavePlayerData(gameData);
			}
		}

		public Color GetGradeColor(Grade grade) => grade switch
		{
			Grade.Common => Color.white,
			Grade.Uncommon => new Color(43 / 255f, 123 / 255f, 1),
			Grade.Rare => new Color(242 / 255f, 210 / 255f, 0),
			Grade.Legendary => new Color(1, 0, 142 / 255f),
			_ => throw new ArgumentOutOfRangeException(nameof(Grade), grade, null)
		};

		private void OnApplicationQuit() => SaveData();

		public EquipmentData GetEquipment(int dollID, int equipmentIndex)
		{
			List<Guid?> guids = GetDoll(dollID).EquipmentGuids;

			if (guids.Count <= equipmentIndex)
				return null;

			return SOManager.ItemInventory
			.GetItem(SOManager.ItemInventory.FindItemIndex(guids[equipmentIndex]))?
			.Data as EquipmentData;
		}
	}

	public interface ISavable<T>
	{
		void Load(T saveData);
		T Save();
	}

	[Serializable]
	public class GameData
	{
		public int curDollIndex = 0;
		public int dummyDollCount = 1;
		public int nyang = 100;

		public List<InventorySlotData> itemInventoryItems = new();
		public List<DollData> dollDataList = new();
		public Dictionary<WorkListType, List<Work>> works = new()
		{
			{ WorkListType.DollWork, new() },
			{ WorkListType.DummyWork, new() },
			{ WorkListType.VQuestWork, new() }
		};
		public List<QuestDataSave> questDataList = new();
		public List<Quest> quests = new();
		public Dictionary<StatisticsType, int> statistics = new();
	}

	[Serializable]
	public struct InventorySlotData
	{
		public int slotIndex;
		public Guid? Guid;
		public int itemID;
		public int itemAmount;

		public InventorySlotData(int slotIndex, Item item)
		{
			this.slotIndex = slotIndex;
			Guid = item.Guid;
			itemID = item.Data.ID;
			itemAmount = item.Amount;
		}
	}

	[Serializable]
	public struct QuestDataSave
	{
		public int QuestID;
		public QuestDataState State;

		public QuestDataSave(int questID, QuestDataState state)
		{
			QuestID = questID;
			State = state;
		}
	}

	[Serializable]
	public struct DollData
	{
		public int DollID;
		public int Level;
		public int Exp;
		public List<Guid?> EquipmentGuids;

		public DollData(int dollID, int dollLevel, int dollExp, List<Guid?> equipmentGuids)
		{
			DollID = dollID;
			Level = dollLevel;
			Exp = dollExp;
			EquipmentGuids = equipmentGuids.ToList();
		}
	}
}