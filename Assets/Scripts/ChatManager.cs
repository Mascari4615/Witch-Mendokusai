using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    private List<ChatData> curChatDatas;
    private int curChatIndex = 0;

    [SerializeField] private UIChatCanvas uiChatCanvas;
    [SerializeField] private CinemachineTargetGroup chatTargetGroup;
    
    public void StartChat(List<ChatData> chatDatas, Transform unitTransform)
    {
        GameManager.Instance.SetPlayerState(PlayerState.Interact);
        curChatDatas = chatDatas;
        chatTargetGroup.m_Targets[1].target = unitTransform;
        
        NextChat();
    }

    public void NextChat()
    {
        if (uiChatCanvas.IsChating)
        {
            uiChatCanvas.SkipChat();
            return;
        }

        if (curChatIndex == curChatDatas.Count)
        {
            EndChat();
            return;
        }

        uiChatCanvas.StartChat(curChatDatas[curChatIndex++]);
    }

    public void EndChat()
    {
        GameManager.Instance.SetPlayerState(PlayerState.Peaceful);
        curChatDatas = null;
        
        // 바로 null로 만드니 블렌드 전에 뚝 끊김
        // 어차피 새로 Chat 시작하면 target을 그 때 설정하니까
        // chatTargetGroup.m_Targets[1].target = null;
        
        curChatIndex = 0;
    }
}
