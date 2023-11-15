using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DataManager : Singleton<DataManager>
{
    public GameData CurGameData { get; private set; }

    [Header("Unit")] public Unit[] units;
    public readonly Dictionary<int, Unit> UnitDic = new();
    
    [Header("Doll")] public DollData[] dolls;
    private readonly Dictionary<int, DollData> _dollDic = new();

    [Header("Item")] public ItemDataBuffer itemDataBuffer;
    public Inventory itemInventory;
    public Inventory equipInventory;
    public readonly Dictionary<int, ItemData> ItemDic = new();
    public readonly Dictionary<int, ItemData> PotionDIc = new();
    private readonly Dictionary<int, ItemData> _commonItemDic = new();
    private readonly Dictionary<int, ItemData> _unCommonItemDic = new();
    private readonly Dictionary<int, ItemData> _rareItemDic = new();
    private readonly Dictionary<int, ItemData> _legendItemDic = new();

    [Header("Stage")] public StageDataBuffer StageDataBuffer;
    public readonly Dictionary<int, Stage> StageDic = new();

    [Header("Mastery")] public MasteryBuffer MasteryBuffer;
    private readonly Dictionary<int, Mastery> _masteryDic = new();

    public readonly Dictionary<string, int> CraftDic = new();

    public Action OnCurGameDataLoad;
    // DataManager.Instance.OnCurGameDataLoad += UpdateVolume;

    public string localDisplayName = "";

    [SerializeField] private PlayFabManager _playFabManager;

    protected override void Awake()
    {
        base.Awake();

        foreach (var item in itemDataBuffer.items)
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

        foreach (var item in itemDataBuffer.items)
        {
            foreach (var recipe in item.Recipes)
            {
                var recipeToList = recipe.Ingredients.Select(ingredient => ingredient.ID).ToList();
                recipeToList.Sort();
                CraftDic.Add(string.Join(',', recipeToList), item.ID);
            }
        }

        foreach (var stage in StageDataBuffer.items)
            StageDic.Add(stage.ID, stage);

        foreach (var unit in units)
            UnitDic.Add(unit.ID, unit);
        foreach (var doll in dolls)
            _dollDic.Add(doll.ID, doll);
        
        foreach (var mastery in MasteryBuffer.items)
            _masteryDic.Add(mastery.ID, mastery);
    }

    [SerializeField] private EquipmentData[] stuffs;

    public void CreateNewGameData()
    {
        CurGameData = new GameData();
        
        itemInventory.InitItems(CurGameData.itemInventoryItems);
        equipInventory.InitItems(CurGameData.equipmentInventoryItems);
        
        equipInventory.Add(stuffs[0], 1);
        equipInventory.Add(stuffs[1], 1);
        equipInventory.Add(stuffs[2], 1);
        equipInventory.Add(stuffs[3], 1);
        
        CurGameData.CurStuffs[0] = equipInventory.GetItem(equipInventory.FindItemSlotIndex(stuffs[0])).Guid;
        CurGameData.CurStuffs[1] = equipInventory.GetItem(equipInventory.FindItemSlotIndex(stuffs[1])).Guid;
        CurGameData.CurStuffs[2] = equipInventory.GetItem(equipInventory.FindItemSlotIndex(stuffs[2])).Guid;
    }

    public void SaveData()
    {
        if (CurGameData == null)
        {
            Debug.Log("?");
            return;
        }

        CurGameData.itemInventoryItems = itemInventory.GetInventoryData();
        CurGameData.equipmentInventoryItems = equipInventory.GetInventoryData();
        _playFabManager.SavePlayerData();
    }

    public void LoadData(GameData saveData)
    {
        CurGameData = saveData;
        itemInventory.InitItems(CurGameData.itemInventoryItems);
        equipInventory.InitItems(CurGameData.equipmentInventoryItems);
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
    public EquipmentData CurStuff(int index) => (equipInventory
        .GetItem(equipInventory.FindEquipmentByGuid(CurGameData.CurStuffs[index]))?
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