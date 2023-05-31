using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public enum CanvasType
    {
        Home,
        BookShelf,
        Inventory,
        PotionCraft
    }
    
    [SerializeField] private GameObject[] canvasList;
    private CanvasType curCanvas = CanvasType.Home;

    public void OpenCanvas(int canvasType) => OpenCanvas((CanvasType)canvasType);
    public void OpenCanvas(CanvasType canvasType)
    {
        curCanvas = canvasType;

        for (int i = 0; i < canvasList.Length; i++)
        {
            canvasList[i].gameObject.SetActive(i == (int)canvasType);
        }
    }

    private void Awake()
    {
        OpenCanvas(CanvasType.Home);
    }
}
