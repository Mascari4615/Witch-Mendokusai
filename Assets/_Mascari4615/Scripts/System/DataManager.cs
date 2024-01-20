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
		private readonly Dictionary<int, DollData> _dollDic = new();
		public readonly Dictionary<int, ItemData> ItemDic = new();
		public readonly Dictionary<int, ItemData> PotionDIc = new();
		private readonly Dictionary<int, ItemData> _commonItemDic = new();
		private readonly Dictionary<int, ItemData> _unCommonItemDic = new();
		private readonly Dictionary<int, ItemData> _rareItemDic = new();
		private readonly Dictionary<int, ItemData> _legendItemDic = new();
		public readonly Dictionary<int, Stage> StageDic = new();
		private readonly Dictionary<int, Mastery> _masteryDic = new();
		public readonly Dictionary<string, int> CraftDic = new();

		public Action OnCurGameDataLoad;
		// DataManager.Instance.OnCurGameDataLoad += UpdateVolume;

		public string localDisplayName = "";

		[SerializeField] private PlayFabManager _playFabManager;

		protected override void Awake()
		{
			base.Awake();

			soManager = SOManager.Instance;

			foreach (var item in soManager.ItemDataBuffer.InitItems)
			{
				ItemDic.Add(item.ID, item);
				switch (item.Grade)
				{
					case Grade.Common:
						_commonItemDic.Add(item.ID, item);
						break;
					case Grade.Uncommon:
						_commonItemDic.Add(item.ID, item);
						break;
					case Grade.Rare:
						_rareItemDic.Add(item.ID, item);
						break;
					case Grade.Legendary:
						_legendItemDic.Add(item.ID, item);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			foreach (var item in soManager.ItemDataBuffer.InitItems)
			{
				foreach (var recipe in item.Recipes)
				{
					var recipeToList = recipe.Ingredients.Select(ingredient => ingredient.ID).ToList();
					recipeToList.Sort();
					CraftDic.Add(string.Join(',', recipeToList), item.ID);
				}
			}

			foreach (var stage in soManager.StageDataBuffer.InitItems)
				StageDic.Add(stage.ID, stage);

			foreach (var unit in soManager.Units)
				UnitDic.Add(unit.ID, unit);
			foreach (var doll in soManager.Dolls)
				_dollDic.Add(doll.ID, doll);
			foreach (var quest in soManager.QuestDataBuffer.InitItems)
				QuestDic.Add(quest.ID, quest);

			foreach (var mastery in soManager.MasteryDataBuffer.InitItems)
				_masteryDic.Add(mastery.ID, mastery);
		}

		[SerializeField] private EquipmentData[] stuffs;

		public void CreateNewGameData()
		{
			CurGameData = new GameData();

			soManager.ItemInventory.InitItems(CurGameData.itemInventoryItems);
			soManager.EquipInventory.InitItems(CurGameData.equipmentInventoryItems);

			soManager.EquipInventory.Add(stuffs[0], 1);
			soManager.EquipInventory.Add(stuffs[1], 1);
			soManager.EquipInventory.Add(stuffs[2], 1);
			soManager.EquipInventory.Add(stuffs[3], 1);

			CurGameData.CurStuffs[0] = soManager.EquipInventory.GetItem(soManager.EquipInventory.FindItemSlotIndex(stuffs[0])).Guid;
			CurGameData.CurStuffs[1] = soManager.EquipInventory.GetItem(soManager.EquipInventory.FindItemSlotIndex(stuffs[1])).Guid;
			CurGameData.CurStuffs[2] = soManager.EquipInventory.GetItem(soManager.EquipInventory.FindItemSlotIndex(stuffs[2])).Guid;
		}

		public void SaveData()
		{
			if (CurGameData == null)
			{
				Debug.Log("?");
				return;
			}

			CurGameData.itemInventoryItems = soManager.ItemInventory.GetInventoryData();
			CurGameData.equipmentInventoryItems = soManager.EquipInventory.GetInventoryData();
			_playFabManager.SavePlayerData();
		}

		public void LoadData(GameData saveData)
		{
			CurGameData = saveData;
			soManager.ItemInventory.InitItems(CurGameData.itemInventoryItems);
			soManager.EquipInventory.InitItems(CurGameData.equipmentInventoryItems);
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
			Grade.Common => _commonItemDic.ElementAt(Random.Range(0, _commonItemDic.Count)).Value.ID,
			Grade.Uncommon => _unCommonItemDic.ElementAt(Random.Range(0, _unCommonItemDic.Count)).Value.ID,
			Grade.Rare => _rareItemDic.ElementAt(Random.Range(0, _rareItemDic.Count)).Value.ID,
			Grade.Legendary => _legendItemDic.ElementAt(Random.Range(0, _legendItemDic.Count)).Value.ID,
			_ => throw new ArgumentOutOfRangeException(nameof(Grade), grade, null)
		};

		private void OnApplicationQuit() => SaveData();

		public DollData CurDoll => _dollDic[CurGameData.lastDollIndex];

		[CanBeNull]
		public EquipmentData CurStuff(int index) => (soManager.EquipInventory
			.GetItem(soManager.EquipInventory.FindEquipmentByGuid(CurGameData.CurStuffs[index]))?
			.Data as EquipmentData);
	}

	[Serializable]
	public class GameData
	{
		public List<InventorySlotData> itemInventoryItems = new List<InventorySlotData>();
		public List<InventorySlotData> equipmentInventoryItems = new List<InventorySlotData>();
		public int lastDollIndex;
		public MyDollData[] myDollDatas = new MyDollData[10];
		public Guid?[] CurStuffs = new Guid?[3];
		public int[] curStageIndex = Enumerable.Repeat(0, 10).ToArray();
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
		public Guid Guid;
		public int itemID;
		public int itemAmount;
	}

	[Serializable]
	public struct MyDollData
	{
		public MyDollData(int dollLevel, int dollExp)
		{
			this.dollLevel = dollLevel;
			this.dollExp = dollExp;
		}

		public int dollLevel;
		public int dollExp;
	}
}