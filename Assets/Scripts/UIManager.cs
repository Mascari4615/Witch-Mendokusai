using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public enum MenuPanelType
    {
        Home,
        BookShelf,
        Inventory,
        PotionCraft
    }

    [SerializeField] private GameObject[] canvasList;
    [SerializeField] private GameObject[] menuPanelList;
    private PlayerState _curCanvas = PlayerState.Peaceful;
    private MenuPanelType _curMenuPanel = MenuPanelType.Home;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    public void OpenCanvas(int canvasType) => OpenCanvas((PlayerState)canvasType);
    public void OpenCanvas(PlayerState canvasType)
    {
        Debug.Log($"{nameof(OpenCanvas)}, {canvasType}");
        _curCanvas = canvasType;

        for (var i = 0; i < canvasList.Length; i++)
        {
            canvasList[i].gameObject.SetActive(i == (int)canvasType);
        }
    }
    
    public void OpenPanel(int canvasType) => OpenPanel((PlayerState)canvasType);
    public void OpenPanel(PlayerState canvasType)
    {
        Debug.Log($"{nameof(OpenCanvas)}, {canvasType}");
        _curCanvas = canvasType;

        for (var i = 0; i < canvasList.Length; i++)
        {
            canvasList[i].gameObject.SetActive(i == (int)canvasType);
        }
    }


    private void Start()
    {
        OpenCanvas(PlayerState.Peaceful);
        InitVolumeSliderValue();
        SetMenuActive(false);
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

    [SerializeField] private Toggle framerateToggle;
    
    public void ToggleFramerate()
    {
        Application.targetFrameRate = framerateToggle.isOn ? 60 : 30;
    }

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject menuButton;

    public void ToggleMenu()
    {
        SetMenuActive(!menuPanel.activeSelf);
    }

    public void SetMenuActive(bool active)
    {
        menuPanel.SetActive(active);
        menuButton.transform.rotation = Quaternion.Euler(0, 0, active? -180 : 0);
    }
}
