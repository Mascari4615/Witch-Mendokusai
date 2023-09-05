using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpObject : MonoBehaviour
{
    // 
    [SerializeField] private IntVariable curExp;
    [SerializeField] private int amount;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curExp.RuntimeValue += amount;
            gameObject.SetActive(false);
        }
    }
}
