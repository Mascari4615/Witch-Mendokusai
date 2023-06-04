using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public enum CanvasType
    {
        Home,
        BookShelf,
        Inventory,
        PotionCraft
    }

    public static CanvasManager Instance => instance;
    private static CanvasManager instance;
    
    [SerializeField] private GameObject[] canvasList;
    private CanvasType curCanvas = CanvasType.Home;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private HPBar _hpBar;
    public HPBar HpBar => _hpBar;

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
        instance = this;
    }

    private void Start()
    {
        OpenCanvas(CanvasType.Home);
        InitVolumeSliderValue();
    }

    private void InitVolumeSliderValue()
    {
        masterVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.BusType.Master);
        bgmVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.BusType.BGM);
        sfxVolumeSlider.value = AudioManager.Instance.GetVolume(AudioManager.BusType.SFX);
    }

    public void UpdateVolume(int busType) => UpdateVolume((AudioManager.BusType)busType);
    public void UpdateVolume(AudioManager.BusType busType)
    {
        switch (busType)
        {
            case AudioManager.BusType.Master:
                AudioManager.Instance.SetVolume(AudioManager.BusType.Master, masterVolumeSlider.value);
                break;
            case AudioManager.BusType.BGM:
                AudioManager.Instance.SetVolume(AudioManager.BusType.BGM, bgmVolumeSlider.value);
                break;
            case AudioManager.BusType.SFX:
                AudioManager.Instance.SetVolume(AudioManager.BusType.SFX, sfxVolumeSlider.value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(busType), busType, null);
        }
    }
}
