using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Chatable : InteractiveObject
{
    [SerializeField] private TextAsset chatScripts;
    private Dictionary<string, List<ChatData>> chatDataDic = new Dictionary<string, List<ChatData>>();

    public override void Interact()
    {
        TryGetChatData("테스트", out List<ChatData> chatDatas);
        ChatManager.Instance.StartChat(chatDatas, transform);
    }
    
    public bool TryGetChatData(string eventName, out List<ChatData> chatDatas)
    {
        if (chatDataDic.Count == 0)
            InitChatDic();

        return chatDataDic.TryGetValue(eventName, out chatDatas);
    }

    private void InitChatDic()
    {
        if (chatScripts.bytes[0] == 0xEF && chatScripts.bytes[1] == 0xBB && chatScripts.bytes[2] == 0xBF)
            Debug.Log("It's BOM");
        
        // var bytes = Encoding.GetEncoding(1252).GetBytes(chatScripts.text);
        var myString = Encoding.UTF8.GetString(chatScripts.bytes);
        
        Debug.Log(myString);
        
        var csvText = myString.Substring(0, chatScripts.text.Length - 1);
        var rows = csvText.Split(new[] { '\n' });

        var eventName = string.Empty;
        var chatDatas = new List<ChatData>();
        
        for (int i = 1; i < rows.Length; i++)
        {
            var columns = rows[i].Split(',');

            if (columns[0] == "end")
            {
                chatDataDic.Add(eventName, chatDatas);
                continue;
            }

            if (columns[0] != string.Empty)
            {
                eventName = columns[0];
                chatDatas = new List<ChatData>();
            }

            var chatData = new ChatData(ref columns);
            chatDatas.Add(chatData);
        }
    }
}

public struct ChatData
{
    public int unitID;
    public string chat;
    public string additionalData;

    public ChatData(string unitID, string chat, string additionalData)
    {
        this.unitID = int.Parse(unitID);
        this.chat = chat;
        this.additionalData = additionalData;
    }

    public ChatData(ref string[] columns)
    {
        this.unitID = int.Parse(columns[1]);
        this.chat = columns[2];
        this.additionalData = columns[3];
    }
}