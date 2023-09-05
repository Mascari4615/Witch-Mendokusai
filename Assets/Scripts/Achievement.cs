using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(Achievement), menuName = "Variable/Page")]
public class Achievement : Artifact
{
    public bool Unlock => unlock;
    public Criteria Criteria => criteria;
    
    [SerializeField] private bool unlock;
    [SerializeField] private Criteria criteria;
}
