using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    private List<LineData> curChatDatas;
    private int curChatIndex = 0;

    [SerializeField] private UIChatCanvas uiChatCanvas;
    [SerializeField] private CinemachineTargetGroup chatTargetGroup;

    [SerializeField] private BoolVariable isChatting;
    
    public void StartChat(List<LineData> chatDatas, Transform unitTransform)
    {
        isChatting.RuntimeValue = true;
        GameManager.Instance.SetPlayerState(PlayerState.Interact);
        curChatDatas = chatDatas;
        chatTargetGroup.m_Targets[1].target = unitTransform;
        
        NextChat();
    }

    public void NextChat()
    {
        if (uiChatCanvas.IsPrinting)
        {
            uiChatCanvas.SkipLine();
            return;
        }

        if (curChatIndex == curChatDatas.Count)
        {
            EndChat();
            return;
        }

        uiChatCanvas.StartLine(curChatDatas[curChatIndex++]);
    }

    public void EndChat()
    {
        isChatting.RuntimeValue = false;
        GameManager.Instance.SetPlayerState(PlayerState.Peaceful);
        curChatDatas = null;
        
        // 바로 null로 만드니 블렌드 전에 뚝 끊김
        // 어차피 새로 Chat 시작하면 target을 그 때 설정하니까
        // chatTargetGroup.m_Targets[1].target = null;
        
        curChatIndex = 0;
    }
}