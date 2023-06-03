using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] _virtualCameras;

    public void SetCamera(int index)
    {
        for (int i = 0; i < _virtualCameras.Length; i++)
        {
            _virtualCameras[i].Priority = i == index ? 10 : 0;
        }
    }
}
