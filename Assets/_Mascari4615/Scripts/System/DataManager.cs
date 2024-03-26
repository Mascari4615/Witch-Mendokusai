using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json;

namespace Mascari4615
{
	public class DataManager : Singleton<DataManager>
	{
		private SOManager SOManager;

		public readonly Dictionary<int, QuestData> QuestDic = new();
		public readonly Dictionary<int, Doll> DollDic = new();
		public readonly Dictionary<int, Unit> UnitDic = new();
		public readonly Dictionary<int, Dungeon> DungeonDic = new();
		public readonly Dictionary<int, ItemData> ItemDic = new();
		public readonly Dictionary<int, ItemData> PotionDic = new();
		public readonly Dictionary<int, ItemData> CommonItemDic = new();
		public readonly Dictionary<int, ItemData> UncommonItemDic = new();
		public readonly Dictionary<int, ItemData> RareItemDic = new();
		public readonly Dictionary<int, ItemData> LegendItemDic = new();
		public readonly Dictionary<int, Stage> StageDic = new();
		public readonly Dictionary<int, Card> CardDic = new();
		public readonly Dictionary<string, int> CraftDic = new();
		public readonly Dictionary<int, string> StatDic = new();

		public bool IsInited { get; private set; }
		public int CurDollID;
		public int DummyDollCount;

		public string localDisplayName = "";

		public PlayFabManager PlayFabManager { get; private set; }
		public WorkManager WorkManager { get; private set; }
		public QuestManager QuestManager { get; private set; }

		// HACK
		[field: SerializeField] public bool UseLocalData { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			SOManager = SOManager.Instance;
			PlayFabManager = GetComponent<PlayFabManager>();
			WorkManager = new();
			QuestManager = new();

			foreach (ItemData item in SOManager.Items)
			{
				ItemDic.Add(item.ID, item);
				switch (item.Grade)
				{
					case Grade.Common:
						CommonItemDic.Add(item.ID, item);
						break;
					case Grade.Uncommon:
						UncommonItemDic.Add(item.ID, item);
						break;
					case Grade.Rare:
						RareItemDic.Add(item.ID, item);
						break;
					case Grade.Legendary:
						LegendItemDic.Add(item.ID, item);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			foreach (ItemData item in SOManager.Items)
			{
				if (item.Recipes == null)
					continue;

				foreach (Recipe recipe in item.Recipes)
				{
					List<int> recipeToList = recipe.Ingredients.Select(ingredient => ingredient.ID).ToList();
					recipeToList.Sort();
					CraftDic.Add(string.Join(',', recipeToList), item.ID);
				}
			}

			foreach (Stage stage in SOManager.StageDataBuffer.InitItems)
				StageDic.Add(stage.ID, stage);

			foreach (Doll doll in SOManager.Dolls)
			{
				DollDic.Add(doll.ID, doll);
				UnitDic.Add(doll.ID, doll);
			}
			foreach (NPC npc in SOManager.NPCs)
				UnitDic.Add(npc.ID, npc);
			foreach (Dungeon dungeon in SOManager.Dungeons)
				DungeonDic.Add(dungeon.ID, dungeon);
			foreach (QuestData quest in SOManager.Quests)
				QuestDic.Add(quest.ID, quest);

			foreach (Card card in SOManager.CardDataBuffer.InitItems)
				CardDic.Add(card.ID, card);

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

		private void Start()
		{
			TimeManager.Instance.AddCallback(WorkManager.TickEachWorks);
		}

		public void CreateNewGameData()
		{
			GameData newGameData = new()
			{
				curDollIndex = 0,
				dummyDollCount = 1,
				itemInventoryItems = new(),
				dollDatas = new()
				{
					new(0, 1, 0, new(){})
				},
				works = new()
				{
					{ WorkListType.DollWork, new() },
					{ WorkListType.DummyWork, new() },
					{ WorkListType.VQuestWork, new() }
				},
				questDatas = new(),
				quests = new(),
				statistics = new()
			};

			// 아이템(장비) 초기화
			Doll defaultDoll = DollDic[0];
			defaultDoll.EquipmentGuids.Clear();

			Inventory inventory = SOManager.ItemInventory;
			inventory.Load(newGameData.itemInventoryItems);
			foreach (EquipmentData equipmentData in defaultDoll.EquipmentDatas)
			{
				inventory.Add(equipmentData);
				Guid? guid = inventory.GetItem(inventory.FindItemSlotIndex(equipmentData)).Guid;
				newGameData.dollDatas[0].EquipmentGuids.Add(guid);
				defaultDoll.EquipmentGuids.Add(guid);
			}
			defaultDoll.EquipmentGuids.Add(null);
			newGameData.itemInventoryItems = inventory.Save();

			// 장비 초기화 이후 저장
			foreach (var d in DollDic)
			{
				if (d.Value.ID != 0)
					newGameData.dollDatas.Add(d.Value.Save());
			}
			foreach (var q in QuestDic)
				newGameData.questDatas.Add(q.Value.Save());

			SaveData();
			LoadData(newGameData);
		}

		public void LoadData(GameData saveData)
		{
			CurDollID = saveData.curDollIndex;
			DummyDollCount = saveData.dummyDollCount;

			// 통계 초기화
			SOManager.Statistics.Load(saveData.statistics);

			// 아이템 초기화
			SOManager.ItemInventory.Load(saveData.itemInventoryItems);

			// 인형 초기화
			SOManager.DollBuffer.ClearBuffer();
			foreach (DollData dollData in saveData.dollDatas)
			{
				DollDic[dollData.DollID].Load(dollData);
				SOManager.DollBuffer.AddItem(DollDic[dollData.DollID]);
			}
			for (int i = 0; i < saveData.dummyDollCount - 1; i++)
				SOManager.DollBuffer.AddItem(DollDic[Doll.DUMMY_ID]);

			// 퀘스트 초기화
			foreach (QuestSaveData questData in saveData.questDatas)
			{
				QuestDic[questData.QuestID].Load(questData);
				if (questData.State >= QuestDataState.Unlocked)
					SOManager.QuestDataBuffer.AddItem(QuestDic[questData.QuestID]);
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
				itemInventoryItems = SOManager.ItemInventory.Save(),
				dollDatas = new(),
				works = WorkManager.Works,
				questDatas = new(),
				quests = QuestManager.Quests,
				statistics = SOManager.Statistics.Save()
			};

			foreach (var d in DollDic)
				gameData.dollDatas.Add(d.Value.Save());
			foreach (var q in QuestDic)
				gameData.questDatas.Add(q.Value.Save());

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

		public int GetRandomItemID() =>
			GetRandomItemID((Grade)Random.Range(0, 4));

		public int GetRandomItemID(Grade grade) => grade switch
		{
			Grade.Common => CommonItemDic.ElementAt(Random.Range(0, CommonItemDic.Count)).Value.ID,
			Grade.Uncommon => UncommonItemDic.ElementAt(Random.Range(0, UncommonItemDic.Count)).Value.ID,
			Grade.Rare => RareItemDic.ElementAt(Random.Range(0, RareItemDic.Count)).Value.ID,
			Grade.Legendary => LegendItemDic.ElementAt(Random.Range(0, LegendItemDic.Count)).Value.ID,
			_ => throw new ArgumentOutOfRangeException(nameof(Grade), grade, null)
		};

		private void OnApplicationQuit() => SaveData();

		public EquipmentData GetEquipment(int dollID, int equipmentIndex)
		{
			List<Guid?> guids = DollDic[dollID].EquipmentGuids;

			if (guids.Count <= equipmentIndex)
				return null;

			return SOManager.ItemInventory
			.GetItem(SOManager.ItemInventory.FindEquipmentByGuid(guids[equipmentIndex]))?
			.Data as EquipmentData;
		}

		[ContextMenu(nameof(TestWork))]
		public void TestWork()
		{
			WorkManager.AddWork(new(0, WorkType.QuestWork, new Guid(), 10));
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
		public int curDollIndex;
		public int dummyDollCount = 1;

		public List<InventorySlotData> itemInventoryItems = new();
		public List<DollData> dollDatas = new();
		public Dictionary<WorkListType, List<Work>> works = new()
		{
			{ WorkListType.DollWork, new() },
			{ WorkListType.DummyWork, new() },
			{ WorkListType.VQuestWork, new() }
		};
		public List<QuestSaveData> questDatas = new();
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
	public struct QuestSaveData
	{
		public int QuestID;
		public QuestDataState State;

		public QuestSaveData(int questID, QuestDataState state)
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