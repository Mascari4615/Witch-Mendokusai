using System;
using System.Collections;
using System.Collections.Generic;
using Karmotrine.Script;
using UnityEngine;

public class ClickerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] clickerUIs;
    [SerializeField] private EnemyObject[] enemyObjects;
    [SerializeField] private SpriteRenderer background;
    
    private Stage _curStage;
    private ContentType _curClickerType = ContentType.Home;

    private int CurClickerIndex => (int)_curClickerType - 1;

    private void Awake()
    {
        foreach (var enemyObject in enemyObjects)
            enemyObject.OnEnemyDied += SpawnEnemy;
    }

    public void OpenClicker(ContentType contentType)
    {
        var prevClickerType = _curClickerType;
        _curClickerType = contentType;

        foreach (var clickerUI in clickerUIs)
            clickerUI.SetActive(contentType != ContentType.Home);

        if (contentType == ContentType.Home)
            return;
        
        if (prevClickerType == contentType)
            return;
        
        SetStage();
        SpawnEnemy();
    }

    public void SetStage()
    {
        var curStageIndex = DataManager.Instance.CurGameData.curStageIndex[(int)_curClickerType];
        _curStage = DataManager.Instance.StageDic[_curClickerType][curStageIndex];
        // background.sprite = curStage.background;
    }

    public void SpawnEnemy()
    {
        if (enemyObjects[CurClickerIndex].IsAlive)
        {
            // TODO : Clear Cur Enemy
        }
        
        Probability<Enemy> probability = new();
        foreach (var enemy in _curStage.SpecialThingWithPercentages)
            probability.Add(enemy.Artifact as Enemy, enemy.Percentage);

        var newEnemy = probability.Get();
        enemyObjects[CurClickerIndex].Init(newEnemy);
    }

    public void Click()
    {
        enemyObjects[CurClickerIndex].ReceiveAttack(3);
    }
}
