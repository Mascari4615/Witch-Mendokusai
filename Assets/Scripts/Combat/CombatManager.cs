using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatManager : Singleton<CombatManager>
{
    public enum CombatStage
    {
        MyTurn, // 내 턴
        EnemyTurn, // 상대 턴
        WaitingAnime // 연출
    }

    [SerializeField] private List<UnitInstance> unitInstances;
    [SerializeField] private CombatUIManager combatUIManager;
    public List<UnitInstance> UnitInstances => unitInstances;
    public CombatStage CurState => curState;
    private CombatStage curState = CombatStage.WaitingAnime;
    private UnitInstance curTurnUnit = null;

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
        
        // SetState(CombatStage.WaitingAnime);
        // StartCoroutine(CombatLoop());
        // Invoke(nameof(Next), 3f);
    }

    private IEnumerator CombatLoop()
    {
        var ws01 = new WaitForSeconds(.1f);
        
        while (true)
        {
            
            yield return ws01;
        }
    }

    /*
    private IEnumerator CombatLoop()
    {
        while (true)
        {
            switch (curState)
            {
                case CombatStage.MyTurn:
                    break;
                case CombatStage.EnemyTurn:
                    UseRandomSkill();
                    break;
                case CombatStage.WaitingAnime:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            yield return null;
        }
    }

    public void Next()
    {
        Debug.Log($"{nameof(Next)}");
        // 다음 유닛 체크
        // TDOO : UI Update

        curTurnUnit = null;
        
        while (true)
        {
            foreach (var unitInstance in unitInstances)
            {
                if (unitInstance.IsReady)
                {
                    curTurnUnit = unitInstance;
                    break;
                }
            }

            if (curTurnUnit != null)
            {
                break;
            }
            else
            {
                foreach (var unitInstance in unitInstances)
                {
                    unitInstance.NextTick();
                }
            }
        }

        SetState(curTurnUnit.IsAlly ? CombatStage.MyTurn : CombatStage.EnemyTurn);
        combatUIManager.UpdateTurnUI();
        foreach (var unitInstance in unitInstances)
        {
            unitInstance.StartTurn(unitInstance == curTurnUnit);
        }
    }

    public void OnAnimeEnd()
    {
        Debug.Log($"{nameof(OnAnimeEnd)}");
        Invoke(nameof(Next), 1f);
    }

    public void SetState(CombatStage newState)
    {
        Debug.Log($"{nameof(SetState)}({newState})");
        if (curState == newState)
            return;

        curState = newState;
        combatUIManager.UpdatePanel();
    }
    
    public void UseSkill(int index)
    {
        Debug.Log($"{nameof(UseSkill)}({index})");
        SetState(CombatStage.WaitingAnime);
        
        // HACK
        Invoke(nameof(OnAnimeEnd), 1f);
    }

    public void UseRandomSkill()
    {
        Debug.Log($"{nameof(UseRandomSkill)}");
        SetState(CombatStage.WaitingAnime);
        
        // HACK
        Invoke(nameof(OnAnimeEnd), 1f);
    }
    */
}
