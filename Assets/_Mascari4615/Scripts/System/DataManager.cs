using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class DataManager : Singleton<DataManager>
	{
		private SOManager soManager;

		public readonly Dictionary<int, Quest> QuestDic = new();
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

		public int CurDollID;
		public int DummyDollCount;

		public string localDisplayName = "";

		[SerializeField] private PlayFabManager _playFabManager;
		public WorkManager WorkManager { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			soManager = SOManager.Instance;
			WorkManager = new();

			foreach (ItemData item in soManager.Items)
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

			foreach (ItemData item in soManager.Items)
			{
				foreach (Recipe recipe in item.Recipes)
				{
					List<int> recipeToList = recipe.Ingredients.Select(ingredient => ingredient.ID).ToList();
					recipeToList.Sort();
					CraftDic.Add(string.Join(',', recipeToList), item.ID);
				}
			}

			foreach (Stage stage in soManager.StageDataBuffer.InitItems)
				StageDic.Add(stage.ID, stage);

			foreach (Doll doll in soManager.Dolls)
			{
				DollDic.Add(doll.ID, doll);
				UnitDic.Add(doll.ID, doll);
			}
			foreach (NPC npc in soManager.NPCs)
				UnitDic.Add(npc.ID, npc);
			foreach (Dungeon dungeon in soManager.Dungeons)
				DungeonDic.Add(dungeon.ID, dungeon);
			foreach (Quest quest in soManager.Quests)
				QuestDic.Add(quest.ID, quest);

			foreach (Card card in soManager.CardDataBuffer.InitItems)
				CardDic.Add(card.ID, card);
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
					new(0, 1, 0, new(){null, null, null, null})
				},
				dollWorks = new(),
				dummyWorks = new(),
				questDatas = new()
			};

			// 아이템(장비) 초기화
			Doll defaultDoll = DollDic[0];
			defaultDoll.EquipmentGuids.Clear();

			Inventory inventory = soManager.ItemInventory;
			inventory.LoadSaveItems(newGameData.itemInventoryItems);
			foreach (EquipmentData equipmentData in defaultDoll.EquipmentDatas)
			{
				inventory.Add(equipmentData);
				Guid? guid = inventory.GetItem(inventory.FindItemSlotIndex(equipmentData)).Guid;
				newGameData.dollDatas[0].EquipmentGuids.Add(guid);
				defaultDoll.EquipmentGuids.Add(guid);
			}
			newGameData.itemInventoryItems = inventory.GetInventoryData();

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

			// 아이템 초기화
			soManager.ItemInventory.LoadSaveItems(saveData.itemInventoryItems);

			// 인형 초기화
			soManager.DollBuffer.ClearBuffer();
			foreach (DollData dollData in saveData.dollDatas)
			{
				DollDic[dollData.DollID].Load(dollData);
				soManager.DollBuffer.AddItem(DollDic[dollData.DollID]);
			}
			for (int i = 0; i < saveData.dummyDollCount - 1; i++)
				soManager.DollBuffer.AddItem(DollDic[Doll.DUMMY_ID]);

			// 퀘스트 초기화
			foreach (QuestData questData in saveData.questDatas)
			{
				QuestDic[questData.QuestID].Load(questData);

				if (questData.State >= QuestState.Unlocked)
					soManager.QuestBuffer.AddItem(QuestDic[questData.QuestID]);
			}

			// 작업 초기화
			WorkManager.Init(saveData.dollWorks, saveData.dummyWorks);
		}

		public void SaveData()
		{
			GameData gameData = new()
			{
				curDollIndex = CurDollID,
				dummyDollCount = DummyDollCount,
				itemInventoryItems = soManager.ItemInventory.GetInventoryData(),
				dollDatas = new(),
				dollWorks = WorkManager.DollWorks,
				dummyWorks = WorkManager.DummyWorks,
				questDatas = new()
			};

			foreach (var d in DollDic)
				gameData.dollDatas.Add(d.Value.Save());
			foreach (var q in QuestDic)
				gameData.questDatas.Add(q.Value.Save());

			_playFabManager.SavePlayerData(gameData);
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

			return soManager.ItemInventory
			.GetItem(soManager.ItemInventory.FindEquipmentByGuid(guids[equipmentIndex]))?
			.Data as EquipmentData;
		}

		[ContextMenu(nameof(TestWork))]
		public void TestWork()
		{
			WorkManager.AddWork(new(0, WorkType.CompleteQuest, 0, 10));
		}
	}

	[Serializable]
	public class GameData
	{
		public int curDollIndex;
		public int dummyDollCount = 1;

		public List<InventorySlotData> itemInventoryItems = new();
		public List<DollData> dollDatas = new();
		public List<Work> dollWorks = new();
		public List<Work> dummyWorks = new();
		public List<QuestData> questDatas = new();
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
	public struct QuestData
	{
		public int QuestID;
		public QuestState State;

		public QuestData(int questID, QuestState state)
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