using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatManager : Singleton<CombatManager>
{
    private CombatWave curCombatWave;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private Transform combatPos;

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
}