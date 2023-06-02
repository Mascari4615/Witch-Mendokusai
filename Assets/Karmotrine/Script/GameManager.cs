using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
