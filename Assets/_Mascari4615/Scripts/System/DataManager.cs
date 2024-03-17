using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class DataManager : Singleton<DataManager>
	{
		private SOManager soManager;

		public GameData CurGameData { get; private set; }

		public readonly Dictionary<int, Quest> QuestDic = new();
		public readonly Dictionary<int, Unit> UnitDic = new();
		public readonly Dictionary<int, Doll> DollDic = new();
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

		public Action OnCurGameDataLoad;
		// DataManager.Instance.OnCurGameDataLoad += UpdateVolume;

		public string localDisplayName = "";

		[SerializeField] private PlayFabManager _playFabManager;

		protected override void Awake()
		{
			base.Awake();

			soManager = SOManager.Instance;

			foreach (ItemData item in soManager.ItemDataBuffer.InitItems)
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

			foreach (ItemData item in soManager.ItemDataBuffer.InitItems)
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

			foreach (Unit unit in soManager.Units)
				UnitDic.Add(unit.ID, unit);
			foreach (Doll doll in soManager.Dolls)
				DollDic.Add(doll.ID, doll);
			foreach (Dungeon dungeon in soManager.Dungeons)
				DungeonDic.Add(dungeon.ID, dungeon);
			foreach (Quest quest in soManager.QuestDataBuffer.InitItems)
				QuestDic.Add(quest.ID, quest);

			foreach (Card card in soManager.CardDataBuffer.InitItems)
				CardDic.Add(card.ID, card);
		}

		public void CreateNewGameData()
		{
			Debug.Log(nameof(CreateNewGameData));
			CurGameData = new GameData();

			soManager.ItemInventory.LoadSaveItems(CurGameData.itemInventoryItems);

			for (int i = 0; i < 3; i++)
			{
				EquipmentData equipmentData = DollDic[0].EquipmentDatas[i];
				soManager.ItemInventory.Add(equipmentData);

				CurGameData.myDollDatas[0].equipmentGuids[i] = soManager.ItemInventory.GetItem(soManager.ItemInventory.FindItemSlotIndex(equipmentData)).Guid;
			}
		}

		public void SaveData()
		{
			if (CurGameData == null)
			{
				Debug.Log("?");
				return;
			}

			CurGameData.itemInventoryItems = soManager.ItemInventory.GetInventoryData();
			_playFabManager.SavePlayerData();
		}

		public void LoadData(GameData saveData)
		{
			CurGameData = saveData;
			soManager.ItemInventory.LoadSaveItems(CurGameData.itemInventoryItems);
			// OnCurGameDataLoad.Invoke();
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

		[CanBeNull]
		public EquipmentData GetEquipment(int index)
		{
			return soManager.ItemInventory
			.GetItem(soManager.ItemInventory.FindEquipmentByGuid(CurGameData.myDollDatas[CurGameData.lastDollIndex].equipmentGuids[index]))?
			.Data as EquipmentData;
		}
	}

	[Serializable]
	public class GameData
	{
		public List<InventorySlotData> itemInventoryItems = new();
		public int lastDollIndex;
		public MyDollData[] myDollDatas = new MyDollData[10];
		public int[] curStageIndex = Enumerable.Repeat(0, 10).ToArray();

		public GameData()
		{
			for (int i = 0; i < 10; i++)
				myDollDatas[i] = new MyDollData(1, 0, new Guid?[3] { null, null, null });
		}
	}

	[Serializable]
	public struct InventorySlotData
	{
		public InventorySlotData(int slotIndex, Item item)
		{
			this.slotIndex = slotIndex;
			Guid = item.Guid;
			itemID = item.Data.ID;
			itemAmount = item.Amount;
		}

		public int slotIndex;
		public Guid? Guid;
		public int itemID;
		public int itemAmount;
	}

	[Serializable]
	public struct MyDollData
	{
		public MyDollData(int dollLevel, int dollExp, Guid?[] equipmentGuids)
		{
			this.dollLevel = dollLevel;
			this.dollExp = dollExp;
			this.equipmentGuids = equipmentGuids;
		}

		public int dollLevel;
		public int dollExp;
		public Guid?[] equipmentGuids;
	}
}