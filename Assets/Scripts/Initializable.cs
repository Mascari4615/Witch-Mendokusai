using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializable : MonoBehaviour
{
    [SerializeField] private CombatWave combatWave;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Initiating();
        }
    }

    public void Initiating()
    {
        CombatManager.Instance.Initiating(combatWave);
    }
}
