using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInitiator : MonoBehaviour
{
    [SerializeField] private Dungeon dungeon;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Initiating();
        }
    }

    public void Initiating()
    {
        CombatManager.Instance.StartCombat(dungeon);
    }
}
