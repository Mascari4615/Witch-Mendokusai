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

    public int[] itemCount = Enumerable.Repeat(0, 100).ToArray();
    public int[] potionCount = Enumerable.Repeat(0, 100).ToArray();

    public int[] curStageIndex = Enumerable.Repeat(0, 10).ToArray();
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance
    {
        get => instance
            ? instance
            : FindObjectOfType<DataManager>() ?? Instantiate(Resources.Load<DataManager>(nameof(DataManager)));
        private set => instance = value;
    }

    private static DataManager instance;

    public GameData CurGameData => curGameData;
    private GameData curGameData;

    [Header("Item")]
    public ItemDataBuffer itemDataBuffer;
    public Inventory Inventory;
    public readonly Dictionary<int, ItemData> ItemDic = new();
    public readonly Dictionary<int, ItemData> PotionDIc = new();
    private readonly Dictionary<int, ItemData> commonItemDic = new();
    private readonly Dictionary<int, ItemData> unCommonItemDic = new();
    private readonly Dictionary<int, ItemData> rareItemDic = new();
    private readonly Dictionary<int, ItemData> legendItemDic = new();

    [Header("Stage")]
    public StageDataBuffer CaveStageDataBuffer;
    public StageDataBuffer ForestStageDataBuffer;
    public StageDataBuffer AdventureStageDataBuffer;
    public readonly Dictionary<ContentType, Stage[]> stageDic = new();

    public readonly Dictionary<string, int> craftDic = new();

    public Action OnCurGameDataLoad;
    public string LocalDisplayName = "";

    private PlayFabManager playFabManager;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var item in itemDataBuffer.items)
        {
            ItemDic.Add(item.ID, item);
            switch (item.Grade)
            {
                case Grade.Common:
                    commonItemDic.Add(item.ID, item);
                    break;
                case Grade.Uncommon:
                    unCommonItemDic.Add(item.ID, item);
                    break;
                case Grade.Rare:
                    rareItemDic.Add(item.ID, item);
                    break;
                case Grade.Legendary:
                    legendItemDic.Add(item.ID, item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (var item in itemDataBuffer.items)
        {
            foreach (var recipe in item.Recipes)
            {
                var recipeToList = new List<int>();

                foreach (var ingredient in recipe.ingredients)
                    recipeToList.Add(ingredient.ID);
                
                recipeToList.Sort();
                craftDic.Add(String.Join(',', recipeToList), item.ID);
            }
        }

        stageDic.Add(ContentType.Forest, ForestStageDataBuffer.items);
        stageDic.Add(ContentType.Adventure, AdventureStageDataBuffer.items);
        stageDic.Add(ContentType.Cave, CaveStageDataBuffer.items);

        playFabManager = GetComponent<PlayFabManager>();
    }

    public void CreateNewGameData()
    {
        curGameData = new GameData();
        Inventory.InitItems(curGameData.inventoryItems);
    }

    public void SaveData()
    {
        if (curGameData == null)
        {
            Debug.Log("?");
            return;
        }

        curGameData.inventoryItems = Inventory.GetInventoryData();
        playFabManager.SavePlayerData();
    }

    public void LoadData(GameData saveData)
    {
        curGameData = saveData;
        Inventory.InitItems(curGameData.inventoryItems);

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
        Grade.Common => commonItemDic.ElementAt(Random.Range(0, commonItemDic.Count)).Value.ID,
        Grade.Uncommon => unCommonItemDic.ElementAt(Random.Range(0, unCommonItemDic.Count)).Value.ID,
        Grade.Rare => rareItemDic.ElementAt(Random.Range(0, rareItemDic.Count)).Value.ID,
        Grade.Legendary => legendItemDic.ElementAt(Random.Range(0, legendItemDic.Count)).Value.ID,
        _ => throw new ArgumentOutOfRangeException(nameof(Grade), grade, null)
    };
    
#if UNITY_EDITOR
#else
    // private void OnApplicationQuit() => SaveData();
#endif

    private void OnApplicationQuit() => SaveData();
}