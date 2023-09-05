using System.Collections;
using System.Collections.Generic;
using Karmotrine.Script;
using UnityEngine;

public class AbilityModule : MonoBehaviour
{
    [SerializeField] private IntVariable curExp;
    [SerializeField] private IntVariable maxExp;
    [SerializeField] private IntVariable curLevel;
    [SerializeField] private GameEvent onLevelUp;
    [SerializeField] private GameObject levelUpEffect;

    public void UpdateLevel()
    {
        if (curExp.RuntimeValue >= maxExp.RuntimeValue)
        {
            curExp.RuntimeValue -= maxExp.RuntimeValue;
            maxExp.RuntimeValue += 10;
            curLevel.RuntimeValue++;
            onLevelUp.Raise();
            
            GameObject l = ObjectManager.Instance.PopObject(levelUpEffect);
            l.transform.position = PlayerController.Instance.transform.position;
            l.SetActive(true);
        }
    }
}