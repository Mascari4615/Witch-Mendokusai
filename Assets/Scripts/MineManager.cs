using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MineManager: Singleton<MineManager>
{
    [SerializeField] private Transform stoneParent;
    [SerializeField] private StoneObject stonePrefab;
    
    [SerializeField] private GameObject[] stairs;
    [SerializeField] private new CinemachineVirtualCamera camera;
    [SerializeField] private MiningDoll miningDoll;
    
    private readonly List<StoneObject> _stoneObjects = new List<StoneObject>();
    private Dictionary<Vector2Int, int> _curMineStoneData = new Dictionary<Vector2Int, int>();

    private MineClickerStage _curClickerStage;
    private int _curLevel;
    
    [ContextMenu("Test")]
    public void Test()
    {
        if (GameManager.Instance.CurContent != ContentType.CaveGame)
        {
            GameManager.Instance.SetContent(ContentType.CaveGame);
            EnterMine(0);
        }
        else
        {
            Exit();
            GameManager.Instance.SetContent(ContentType.CaveIdle);
        }
    }

    public void EnterMine(int caveID)
    {
        _curClickerStage = DataManager.Instance.CaveGameStageDic[caveID];
        GenerateCave();
        StartCurStage();
    }

    public void EnterNextLevel()
    {
        
    }

    private void GenerateCave()
    {
        Debug.Log(_curClickerStage);
        // background.sprite = _curStage.Background;

        _curMineStoneData = new Dictionary<Vector2Int, int>();

        GenerateStructures(1);
        GenerateMinerals(_curClickerStage.StoneAmount);

        void GenerateStructures(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                while (true)
                {
                    Vector2Int randomCoordinate = new()
                    {
                        x = Random.Range(-49, 49 + 1),
                        y = Random.Range(-49, 49 + 1)
                    };
                    // 100 - 1(=점 기준) - 1(=좌표 0)
                    // 구조물 크기 1이라고 가정

                    if (IsEmpty(randomCoordinate))
                    {
                        stairs[i].transform.localPosition = new Vector3(randomCoordinate.x, 0, randomCoordinate.y);
                        _curMineStoneData.Add(randomCoordinate, 1);
                        break;
                    }
                }
            }

            // bool NearIsEmpty(Vector2Int coordinate) { }
        }

        void GenerateMinerals(int amount)
        {
            if (_stoneObjects.Count < amount)
            {
                var diff = amount - _stoneObjects.Count;
                for (var i = 0; i < diff; i++)
                {
                    var g = Instantiate(stonePrefab, stoneParent);
                    _stoneObjects.Add(g);
                }
            }

            for (var i = 0; i < _stoneObjects.Count; i++)
            {
                _stoneObjects[i].gameObject.SetActive(i < amount);
                if (i >= amount)
                    continue;

                while (true)
                {
                    Vector2Int randomCoordinate = new()
                    {
                        x = Random.Range(-49, 49 + 1),
                        y = Random.Range(-49, 49 + 1)
                    };
                    // 100 - 1(=점 기준) - 1(=좌표 0)
                    // 구조물 크기 1이라고 가정

                    if (!IsEmpty(randomCoordinate))
                        continue;
                    
                    Probability<StoneData> probability = new();
                    foreach (var stone in _curClickerStage.SpecialThingWithPercentages)
                        probability.Add(stone.Artifact as StoneData, stone.Percentage);

                    var stoneData = probability.Get();

                    _stoneObjects[i].Init(stoneData);
                    _stoneObjects[i].transform.localPosition = new Vector3(randomCoordinate.x, 0, randomCoordinate.y);
                    _curMineStoneData.Add(randomCoordinate, 1);
                    break;
                }
            }
        }

        bool IsEmpty(Vector2Int coordinate)
        {
            return !_curMineStoneData.ContainsKey(coordinate);
        }
    }

    private void StartCurStage()
    {
        camera.Priority = 1000;

        miningDoll.transform.localPosition = Vector3.zero;
        miningDoll.gameObject.SetActive(true);
        miningDoll.SetState(MiningDoll.CaveDollState.Idle);
    }

    public void Exit()
    {
        camera.Priority = -1000;
        miningDoll.gameObject.SetActive(false);
    }
}