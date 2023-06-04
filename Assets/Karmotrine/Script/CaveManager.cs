using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CaveManager : MonoBehaviour
{
    private Stage curStage;
    private Dictionary<Vector2Int, int> curCaveData = new Dictionary<Vector2Int, int>();

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private StoneObject stonePrefab;
    [SerializeField] private Transform stoneParent;
    [SerializeField] private int stoneAmount;
    private List<StoneObject> stoneObjects = new List<StoneObject>();
    [SerializeField] private GameObject[] stairs;

    [SerializeField] private CinemachineVirtualCamera _camera;
    [FormerlySerializedAs("caveUnitMove")] [FormerlySerializedAs("_touchMove")] [SerializeField] private CaveDoll caveDoll;
    
    [ContextMenu("Test")]
    public void Test()
    {
        Enter(0);
    }
    
    public void Enter(int caveID)
    {
        curStage = DataManager.Instance.caveGameStageDic[caveID];
        GenerateCave();
        StartCurStage();
    }

    private void GenerateCave()
    {
        background.sprite = curStage.background;

        curCaveData = new Dictionary<Vector2Int, int>();

        GenerateStructures(1);
        GenerateMinerals(stoneAmount);

        void GenerateStructures(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                while (true)
                {
                    Vector2Int randomCoordinate = new();
                    randomCoordinate.x = Random.Range(-49, 49 + 1);
                    randomCoordinate.y = Random.Range(-49, 49 + 1);
                    // 100 - 1(=점 기준) - 1(=좌표 0)
                    // 구조물 크기 1이라고 가정

                    if (IsEmpty(randomCoordinate))
                    {
                        stairs[i].transform.localPosition = new Vector2(randomCoordinate.x, randomCoordinate.y);
                        curCaveData.Add(randomCoordinate, 1);
                        break;
                    }
                }
            }

            // bool NearIsEmpty(Vector2Int coordinate) { }
        }

        void GenerateMinerals(int amount)
        {
            if (stoneObjects.Count < amount)
            {
                var diff = amount - stoneObjects.Count;
                for (int i = 0; i < diff; i++)
                {
                    var g = Instantiate(stonePrefab, stoneParent);
                    stoneObjects.Add(g);
                }
            }

            for (int i = 0; i < stoneObjects.Count; i++)
            {
                stoneObjects[i].gameObject.SetActive(i < amount);
                if (i >= amount)
                    continue;

                while (true)
                {
                    Vector2Int randomCoordinate = new();
                    randomCoordinate.x = Random.Range(-49, 49 + 1);
                    randomCoordinate.y = Random.Range(-49, 49 + 1);
                    // 100 - 1(=점 기준) - 1(=좌표 0)
                    // 구조물 크기 1이라고 가정

                    if (IsEmpty(randomCoordinate))
                    {
                        Probability<StoneData> probability = new();
                        foreach (var stone in curStage.specialThingWithPercentages)
                            probability.Add(stone.specialThing as StoneData, stone.percentage);
                        var stoneData = probability.Get();

                        stoneObjects[i].Init(stoneData);
                        stoneObjects[i].transform.localPosition = new Vector2(randomCoordinate.x, randomCoordinate.y);
                        curCaveData.Add(randomCoordinate, 1);
                        break;
                    }
                }
            }
        }

        bool IsEmpty(Vector2Int coordinate)
        {
            return !curCaveData.ContainsKey(coordinate);
        }
    }

    private void StartCurStage()
    {
        _camera.Priority = 1000;
        
        CanvasManager.Instance.HpBar.Disable();
        
        caveDoll.transform.localPosition = Vector3.zero;
        caveDoll.gameObject.SetActive(true);
        caveDoll.SetState(CaveDoll.CaveDollState.Idle);
    }

    public void Exit()
    {
        _camera.Priority = -1000;
       
        caveDoll.gameObject.SetActive(false);
    }
}