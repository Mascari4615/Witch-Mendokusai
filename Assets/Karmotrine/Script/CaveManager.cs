using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CaveManager : MonoBehaviour
{
    private Stage _curStage;
    private Dictionary<Vector2Int, int> _curCaveData = new Dictionary<Vector2Int, int>();

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private StoneObject stonePrefab;
    [SerializeField] private Transform stoneParent;
    [SerializeField] private int stoneAmount;
    private readonly List<StoneObject> _stoneObjects = new List<StoneObject>();
    [SerializeField] private GameObject[] stairs;

    [SerializeField] private new CinemachineVirtualCamera camera;

    [SerializeField] private CaveDoll caveDoll;

    [ContextMenu("Test")]
    public void Test()
    {
        Enter(0);
    }

    public void Enter(int caveID)
    {
        _curStage = DataManager.Instance.CaveGameStageDic[caveID];
        GenerateCave();
        StartCurStage();
    }

    private void GenerateCave()
    {
        background.sprite = _curStage.Background;

        _curCaveData = new Dictionary<Vector2Int, int>();

        GenerateStructures(1);
        GenerateMinerals(stoneAmount);

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
                        stairs[i].transform.localPosition = new Vector2(randomCoordinate.x, randomCoordinate.y);
                        _curCaveData.Add(randomCoordinate, 1);
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
                    foreach (var stone in _curStage.SpecialThingWithPercentages)
                        probability.Add(stone.Artifact as StoneData, stone.Percentage);

                    var stoneData = probability.Get();

                    _stoneObjects[i].Init(stoneData);
                    _stoneObjects[i].transform.localPosition = new Vector2(randomCoordinate.x, randomCoordinate.y);
                    _curCaveData.Add(randomCoordinate, 1);
                    break;
                }
            }
        }

        bool IsEmpty(Vector2Int coordinate)
        {
            return !_curCaveData.ContainsKey(coordinate);
        }
    }

    private void StartCurStage()
    {
        camera.Priority = 1000;
        CanvasManager.Instance.HpBar.Disable();

        caveDoll.transform.localPosition = Vector3.zero;
        caveDoll.gameObject.SetActive(true);
        caveDoll.SetState(CaveDoll.CaveDollState.Idle);
    }

    public void Exit()
    {
        camera.Priority = -1000;
        caveDoll.gameObject.SetActive(false);
    }
}