using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class GameData
{
    public List<InventorySlotData> inventoryItems = new List<InventorySlotData>();

    public int[] stoneKillCount = Enumerable.Repeat(-1, 1000).ToArray();

    public int[] curStageIndex = Enumerable.Repeat(0, 10).ToArray();
}

public class DataManager : Singleton<DataManager>
{
    public GameData CurGameData { get; private set; }

    [Header("Unit")] public Unit[] units;
    public readonly Dictionary<int, Unit> UnitDic = new();

    [Header("Item")] public ItemDataBuffer itemDataBuffer;
    public Inventory inventory;
    public readonly Dictionary<int, ItemData> ItemDic = new();
    public readonly Dictionary<int, ItemData> PotionDIc = new();
    private readonly Dictionary<int, ItemData> _commonItemDic = new();
    private readonly Dictionary<int, ItemData> _unCommonItemDic = new();
    private readonly Dictionary<int, ItemData> _rareItemDic = new();
    private readonly Dictionary<int, ItemData> _legendItemDic = new();

    [Header("Stage")] public StageDataBuffer caveIdleStageDataBuffer;
    public MineStageDataBuffer caveGameStageDataBuffer;
    public StageDataBuffer forestStageDataBuffer;
    public StageDataBuffer adventureStageDataBuffer;
    public readonly Dictionary<ContentType, ClickerStage[]> StageDic = new();
    public readonly Dictionary<int, ClickerStage> CaveIdleStageDic = new();
    public readonly Dictionary<int, MineClickerStage> CaveGameStageDic = new();

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

        StageDic.Add(ContentType.ForestIdle, forestStageDataBuffer.items);
        StageDic.Add(ContentType.AdventureIdle, adventureStageDataBuffer.items);
        StageDic.Add(ContentType.CaveIdle, caveIdleStageDataBuffer.items);

        foreach (var caveStageData in caveIdleStageDataBuffer.items)
            CaveIdleStageDic.Add(caveStageData.ID, caveStageData);
        foreach (var caveStageData in caveGameStageDataBuffer.items)
            CaveGameStageDic.Add(caveStageData.ID, caveStageData);

        foreach (var unit in units)
            UnitDic.Add(unit.ID, unit);
        
        foreach (var mastery in MasteryBuffer.items)
            _masteryDic.Add(mastery.ID, mastery);
        
    }

    public void CreateNewGameData()
    {
        CurGameData = new GameData();
        inventory.InitItems(CurGameData.inventoryItems);
    }

    public void SaveData()
    {
        if (CurGameData == null)
        {
            Debug.Log("?");
            return;
        }

        CurGameData.inventoryItems = inventory.GetInventoryData();
        _playFabManager.SavePlayerData();
    }

    public void LoadData(GameData saveData)
    {
        CurGameData = saveData;
        inventory.InitItems(CurGameData.inventoryItems);

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
}