using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance => instance;
    private static DataManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Stage[] stages;
    public int curStageIndex;
}
