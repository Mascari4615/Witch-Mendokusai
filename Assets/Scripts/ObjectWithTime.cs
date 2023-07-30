using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithTime : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    private bool isFirstInvoke = true;

    private void OnEnable()
    {
        if (isFirstInvoke)
        {
            isFirstInvoke = false;
            return;
        }
        Invoke(nameof(DisableObject), duration);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
