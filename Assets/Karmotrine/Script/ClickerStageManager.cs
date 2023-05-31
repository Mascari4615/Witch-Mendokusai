using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerStageManager : MonoBehaviour
{
    [SerializeField] private EnemyObject enemyObject;

    private void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (enemyObject.IsAlive)
        {
            // TODO : Clear Cur Enemy
        }
        
        var curStage = DataManager.Instance.stageDic[DataManager.Instance.curStageIndex];
        
        Probability<Enemy> probability = new();
        foreach (var enemy in curStage.Enemies)
            probability.Add(enemy.specialThing as Enemy, enemy.percentage);

        var newEnemy = probability.Get();
        enemyObject.Init(newEnemy);
    }
}
