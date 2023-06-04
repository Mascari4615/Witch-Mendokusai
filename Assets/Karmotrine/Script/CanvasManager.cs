using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    public enum CanvasType
    {
        Home,
        BookShelf,
        Inventory,
        PotionCraft
    }

    [SerializeField] private GameObject[] canvasList;
    private CanvasType _curCanvas = CanvasType.Home;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private HPBar hpBar;
    public HPBar HpBar => hpBar;

    public void OpenCanvas(int canvasType) => OpenCanvas((CanvasType)canvasType);
    public void OpenCanvas(CanvasType canvasType)
    {
        _curCanvas = canvasType;

        for (var i = 0; i < canvasList.Length; i++)
        {
            canvasList[i].gameObject.SetActive(i == (int)canvasType);
        }
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
