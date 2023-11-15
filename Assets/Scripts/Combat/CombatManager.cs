using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatManager : Singleton<CombatManager>
{
    [SerializeField] private MasteryManager masteryManager;
    [SerializeField] private MonsterSpawner monsterSpawner;

    [SerializeField] private UIDungeonEntrance uiDungeonEntrance;
    [SerializeField] private UICombatCanvas uiCombatCanvas;
    [SerializeField] private UIDamage uiDamage;

    [SerializeField] private DateTimeVarialbe combatStartTime;

    public void OpenDungeonEntranceUI(List<Dungeon> dungeonDatas)
    {
        Debug.Log(nameof(OpenDungeonEntranceUI));
        uiDungeonEntrance.OpenCanvas(dungeonDatas);
    }

    public void PopDamage(Vector3 pos, int damge)
    {
        StartCoroutine(uiDamage.DamageTextUI(pos, damge));
    }
    
    public void CombatIntro()
    {
    }

    [ContextMenu(nameof(StartCombat))]
    public void StartCombat(Dungeon dungeon)
    {
        Debug.Log($"{nameof(StartCombat)}");
        
        StageManager.Instance.LoadStage(dungeon, 0);
        monsterSpawner.transform.position = PlayerController.Instance.transform.position;
        GameManager.Instance.SetPlayerState(PlayerState.Combat);

        combatStartTime.RuntimeValue = DateTime.Now;
        
        monsterSpawner.StartWave(dungeon);
        
        uiCombatCanvas.InitTime();
        uiCombatCanvas.UpdateExp();
        uiCombatCanvas.SetActive(true);
        
        masteryManager.Init();

        foreach (var effect in DataManager.Instance.CurStuff(0)!.Effects)
            effect.OnEquip();

        foreach (var effect in DataManager.Instance.CurStuff(1)!.Effects)
            effect.OnEquip();

        foreach (var effect in DataManager.Instance.CurStuff(2)!.Effects)
            effect.OnEquip();
    }

    private IEnumerator CombatLoop()
    {
        var ws01 = new WaitForSeconds(.1f);

        while (true)
        {
            yield return ws01;
        }
    }

    public void EndCombat()
    {
        Debug.Log($"{nameof(EndCombat)}");

        monsterSpawner.StopLoop();

        for (int i = GameManager.Instance.EnemyRuntimeSet.Items.Count - 1; i >= 0; i--)
            GameManager.Instance.EnemyRuntimeSet.Items[i].gameObject.SetActive(false);

        Debug.Log(StageManager.Instance.LastStage);
        StageManager.Instance.LoadStage(StageManager.Instance.LastStage, -1);
        
        GameManager.Instance.SetPlayerState(PlayerState.Peaceful);
        uiCombatCanvas.SetActive(false);
    }
}