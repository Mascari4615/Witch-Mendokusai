using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContentType
{
    Home,
    Cave,
    Forest,
    Adventure,
}

public class GameManager : Singleton<GameManager>
{
    private ContentType _curContent = ContentType.Home;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ClickerManager clickerManager;

    private void Start()
    {
        SetContent(ContentType.Home);
    }

    public void SetContent(int newContent) => SetContent((ContentType)newContent);
    public void SetContent(ContentType newContent)
    {
        _curContent = newContent;
        cameraManager.SetCamera((int)_curContent);
        clickerManager.OpenClicker(newContent);
    }
}
