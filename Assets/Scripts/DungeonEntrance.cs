using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : InteractiveObject
{
    [SerializeField] private List<Dungeon> dungeonDatas;
    
    public override void Interact()
    {
        Debug.Log(nameof(DungeonEntrance));
        CombatManager.Instance.OpenDungeonEntranceUI(dungeonDatas);
    }
}