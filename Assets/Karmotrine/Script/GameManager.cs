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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get => instance
            ? instance
            : FindObjectOfType<GameManager>() ?? Instantiate(Resources.Load<GameManager>(nameof(GameManager)));
        private set => instance = value;
    }
    private static GameManager instance;

    private ContentType _curContent = ContentType.Home;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ClickerManager clickerManager;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

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
