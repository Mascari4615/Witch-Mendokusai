using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerManager : MonoBehaviour
{
    public enum ClickerType
    {
        Cave,
        Forest,
        Adventure,
        None,
    }
    
    [SerializeField] private EnemyObject enemyObject;
    [SerializeField] private SpriteRenderer background;
    private Stage curStage;
    private ClickerType curClickerType = ClickerType.None;

    public void OpenClicker(int clickerType) => OpenClicker((ClickerType)clickerType);
    public void OpenClicker(ClickerType clickerType)
    {
        if (curClickerType == clickerType)
            return;

        curClickerType = clickerType;
        
        SetStage();
        SpawnEnemy();
    }

    public void SetStage()
    {
        int curStageIndex = DataManager.Instance.CurGameData.curStageIndex[(int)curClickerType];
        curStage = DataManager.Instance.stageDic[curClickerType][curStageIndex];
        background.sprite = curStage.background;
    }

    public void SpawnEnemy()
    {
        if (enemyObject.IsAlive)
        {
            // TODO : Clear Cur Enemy
        }
        
        Probability<Enemy> probability = new();
        foreach (var enemy in curStage.Enemies)
            probability.Add(enemy.specialThing as Enemy, enemy.percentage);

        var newEnemy = probability.Get();
        enemyObject.Init(newEnemy);
    }

    public void Click()
    {
        enemyObject.ReceiveAttack(3);
    }
}
