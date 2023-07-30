using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;

    public void SetCamera(int index)
    {
        cinemachineBrain.m_DefaultBlend.m_Style = index == (int)PlayerState.Combat ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseInOut;
        
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].Priority = i == index ? 10 : 0;
        }
    }
}
