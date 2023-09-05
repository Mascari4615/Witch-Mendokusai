using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatManager : Singleton<CombatManager>
{
    private CombatWave curCombatWave;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private Transform combatPos;

    [SerializeField] private UICombatCanvas uiCombatCanvas;
    [SerializeField] private UIDamage uiDamage;

    public void PopDamage(Vector3 pos, int damge)
    {
        StartCoroutine(uiDamage.DamageTextUI(pos, damge));
    }

    public void Initiating(CombatWave combatWave)
    {
        curCombatWave = combatWave;

        monsterSpawner.StartSpawn(curCombatWave.WaveEnemy);
        StartCombat();
    }

    public void CombatIntro()
    {
    }

    [ContextMenu(nameof(StartCombat))]
    public void StartCombat()
    {
        Debug.Log($"{nameof(StartCombat)}");
        
        uiCombatCanvas.InitTime();
        uiCombatCanvas.SetActive(true);
        PlayerController.Instance.TeleportTo(combatPos.position);
        GameManager.Instance.SetPlayerState(PlayerState.Combat);
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
        
        monsterSpawner.StopSpawn();

        for (int i = GameManager.Instance.EnemyRuntimeSet.Items.Count - 1; i >= 0; i--)
            GameManager.Instance.EnemyRuntimeSet.Items[i].gameObject.SetActive(false);
        
        PlayerController.Instance.TeleportTo(Vector3.zero);
        GameManager.Instance.SetPlayerState(PlayerState.Peaceful);
        uiCombatCanvas.SetActive(false);
    }
}