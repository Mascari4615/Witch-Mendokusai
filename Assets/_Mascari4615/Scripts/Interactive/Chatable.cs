using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mascari4615
{
	public class Chatable : InteractiveObject
	{
		[SerializeField] private TextAsset chatScripts;
		private readonly Dictionary<string, List<LineData>> chatDataDic = new();

		public override void Interact()
		{
			if (TryGetChatData("테스트", out List<LineData> chatDatas))
				ChatManager.Instance.StartChat(chatDatas, transform);
		}

		public bool TryGetChatData(string eventName, out List<LineData> chatDatas)
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
			// var myString = Encoding.UTF8.GetString(chatScripts.bytes);
			var myString = chatScripts.text;

			Debug.Log(myString);

			var csvText = myString[..(chatScripts.text.Length - 1)];
			var rows = csvText.Split(new[] { '\n' });

			var eventName = string.Empty;
			var lineDatas = new List<LineData>();

			for (int i = 1; i < rows.Length; i++)
			{
				var columns = rows[i].Split(',');

				if (columns[0] == "end")
				{
					var _chatData = new LineData(ref columns);
					lineDatas.Add(_chatData);

					chatDataDic.Add(eventName, lineDatas);
					continue;
				}

				if (columns[0] != string.Empty)
				{
					eventName = columns[0];
					lineDatas = new List<LineData>();
				}

				var chatData = new LineData(ref columns);
				lineDatas.Add(chatData);
			}
		}
	}

	public struct LineData
	{
		public int unitID;
		public string line;
		public string additionalData;

		public LineData(string unitID, string line, string additionalData)
		{
			this.unitID = int.Parse(unitID);
			this.line = line;
			this.additionalData = additionalData;
		}

		public LineData(ref string[] columns)
		{
			this.unitID = (columns[1] == string.Empty)
				? -1
				: int.Parse(columns[1]);

			this.line = columns[2];

			this.additionalData = (columns[3] == string.Empty)
				? string.Empty
				: columns[3].TrimEnd('\r', '\n', ' ');
		}
	}
}