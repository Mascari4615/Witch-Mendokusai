using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHulahoop : MonoBehaviour
{
    [SerializeField] private StatDictionary statDictionary;
    [SerializeField] private GameObject[] satellites;
    [SerializeField] private float rotateSpeed = 3;
    
    private void OnEnable()
    {
        Transform parent = PlayerController.Instance.transform;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        UpdateEquipment();
    }  
    
    public void UpdateEquipment()
    {
        int satelliteCount = 1 + statDictionary.GetStat("SATELLITE_COUNT");
        float delta = 360f / satelliteCount;

        for (int i = 0; i < satellites.Length; i++)
        {
            satellites[i].SetActive(satelliteCount > i);
            satellites[i].transform.localRotation = Quaternion.Euler(Vector3.up * (delta * i));
        }
    }

    private void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
