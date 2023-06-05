using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContentType
{
    Home,
    CaveIdle,
    ForestIdle,
    AdventureIdle,
    CaveGame,
}

public class GameManager : Singleton<GameManager>
{
    public ContentType CurContent { get; private set; } = ContentType.Home;

    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ClickerManager clickerManager;

    private void Start()
    {
        SetContent(ContentType.Home);
    }

    public void SetContent(int newContent) => SetContent((ContentType)newContent);
    public void SetContent(ContentType newContent)
    {
        CurContent = newContent;
        cameraManager.SetCamera((int)CurContent);
        clickerManager.TryOpenClicker(newContent);
    }
}
