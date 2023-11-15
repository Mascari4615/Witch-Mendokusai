using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject : MonoBehaviour
{
    public Portal[] Portals => portals;
    [SerializeField] private Portal[] portals;

    private void OnEnable()
    {
        foreach (var portal in portals)
            portal.Active();
    }
}
