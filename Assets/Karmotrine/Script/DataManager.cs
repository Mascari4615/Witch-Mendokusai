using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable] public class GameData
{
    public int[] itemCount = Enumerable.Repeat(0, 100).ToArray();
    public int[] potionCount = Enumerable.Repeat(0, 100).ToArray();
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
    
    public int curStageIndex;
    
    [Header("Item")] public ItemDataBuffer itemDataBuffer;
    public readonly Dictionary<int, Item> ItemDic = new();
    [FormerlySerializedAs("wgItemInven")] public PlayerInventory playerInventory;
    private readonly Dictionary<int, Item> commonItemDic = new();
    private readonly Dictionary<int, Item> unCommonItemDic = new();
    private readonly Dictionary<int, Item> rareItemDic = new();
    private readonly Dictionary<int, Item> legendItemDic = new();
    
    [Header("Stage")] public StageDataBuffer stageDataBuffer;
    public readonly Dictionary<int, Stage> stageDic = new();
    
    public Action OnCurGameDataLoad;

    public PlayFabManager PlayFabManager;

    public string LocalDisplayName = "";

    public readonly Dictionary<string, string[]> diedCommentDic = new();
    
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Item item in itemDataBuffer.items)
        {
            ItemDic.Add(item.ID, item);
            switch (item.grade)
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
            }
        }

        foreach (Stage stage in stageDataBuffer.items) stageDic.Add(stage.ID, stage);
        
        PlayFabManager = GetComponent<PlayFabManager>();
    }

    public void CreateNewGameData()
    {
        curGameData = new GameData();
    }

    public void SetGameData(GameData gameData)
    {
        this.curGameData = gameData;
        
        Debug.Log(OnCurGameDataLoad == null);
        // OnCurGameDataLoad.Invoke();
    }

    public Color GetGradeColor(Grade grade) => grade switch
    {
        Grade.Common => Color.white,
        Grade.Uncommon => new(43 / 255f, 123 / 255f, 1),
        Grade.Rare => new(242 / 255f, 210 / 255f, 0),
        Grade.Legendary => new(1, 0, 142 / 255f),
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
}
