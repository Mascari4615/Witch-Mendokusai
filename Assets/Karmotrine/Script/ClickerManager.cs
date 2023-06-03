using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerManager : MonoBehaviour
{
    [SerializeField] private GameObject clickerButton;
    [SerializeField] private EnemyObject[] enemyObjects;
    [SerializeField] private SpriteRenderer background;
    private Stage curStage;
    private ContentType curClickerType = ContentType.Home;

    private int curClickerIndex => (int)curClickerType - 1;
    
    public void OpenClicker(ContentType contentType)
    {
        var prevClickerType = curClickerType;
        curClickerType = contentType;
        
        clickerButton.SetActive(contentType != ContentType.Home);

        if (contentType == ContentType.Home)
            return;
        
        if (prevClickerType == contentType)
            return;
        
        SetStage();
        SpawnEnemy();
    }

    public void SetStage()
    {
        int curStageIndex = DataManager.Instance.CurGameData.curStageIndex[(int)curClickerType];
        curStage = DataManager.Instance.stageDic[curClickerType][curStageIndex];
        // background.sprite = curStage.background;
    }

    public void SpawnEnemy()
    {
        if (enemyObjects[curClickerIndex].IsAlive)
        {
            // TODO : Clear Cur Enemy
        }
        
        Probability<Enemy> probability = new();
        foreach (var enemy in curStage.Enemies)
            probability.Add(enemy.specialThing as Enemy, enemy.percentage);

        var newEnemy = probability.Get();
        enemyObjects[curClickerIndex].Init(newEnemy);
    }

    public void Click()
    {
        enemyObjects[curClickerIndex].ReceiveAttack(3);
    }
}
