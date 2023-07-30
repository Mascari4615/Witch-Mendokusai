using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChatCanvas : MonoBehaviour
{
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI chatText;

    private ChatData chatData;
    private Coroutine chatLoop;
    private WaitForSeconds ws01 = new WaitForSeconds(.1f);

    public bool IsChating { get; private set; } = false;

    public void StartChat(ChatData chatData)
    {
        if (chatLoop != null)
            StopCoroutine(chatLoop);

        var unit = DataManager.Instance.UnitDic[chatData.unitID];
        
        // TODO : 유닛 이미지 바리에이션 어떻게 저장하고 불러온 것인지?
        
        unitImage.sprite = unit.Thumbnail;
        unitName.text = unit.Name;
        chatText.text = string.Empty;

        this.chatData = chatData;
        
        chatLoop = StartCoroutine(ChatLoop());
    }

    public IEnumerator ChatLoop()
    {
        IsChating = true;

        foreach (var c in chatData.chat)
        {
            chatText.text += c;
            RuntimeManager.PlayOneShot("event:/SFX/Equip");
            yield return ws01;
        }
        
        IsChating = false;
    }

    public void SkipChat()
    {
        if (IsChating == false)
            return;
        
        if (chatLoop != null)
            StopCoroutine(chatLoop);

        chatText.text = chatData.chat;

        IsChating = false;
    }
}
