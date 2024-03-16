using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIChat : UIPanel
	{
		private List<LineData> curChatDatas;
		private int curChatIndex = 0;

		[SerializeField] private CinemachineTargetGroup chatTargetGroup;
		[SerializeField] private Image unitImage;
		[SerializeField] private TextMeshProUGUI unitName;
		[SerializeField] private TextMeshProUGUI lineText;

		private LineData _lineData;
		private Coroutine lineLoop;
		private readonly WaitForSecondsRealtime ws01 = new(.1f);

		public bool IsPrinting { get; private set; } = false;

		public override void UpdateUI(int[] someData = null)
		{
			if (someData == null)
				return;

			if (TryGetChatData("테스트", out curChatDatas) == false)
				return;

			SOManager.Instance.IsChatting.RuntimeValue = true;
			// chatTargetGroup.m_Targets[1].target = unitTransform;

			NextChat();
			StartCoroutine(CheckInput());
		}

		public IEnumerator CheckInput()
		{
			while (true)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					NextChat();
				}

				yield return null;
			}
		}

		public void NextChat()
		{
			if (IsPrinting)
			{
				SkipLine();
				return;
			}

			if (curChatIndex == curChatDatas.Count - 1)
			{
				if (curChatDatas[^1].additionalData.Equals("0"))
				{
					Debug.Log("Hawawaaaaaaaaaaaaaaaaaaaaaaaaa");
					// UIManager.Instance.OpenShopPanel();
				}
				EndChat();
				return;
			}

			StartLine(curChatDatas[curChatIndex++]);
		}

		public void EndChat()
		{
			SOManager.Instance.IsChatting.RuntimeValue = false;
			curChatDatas = null;

			StopAllCoroutines();

			// 바로 null로 만드니 블렌드 전에 뚝 끊김
			// 어차피 새로 Chat 시작하면 target을 그 때 설정하니까
			// chatTargetGroup.m_Targets[1].target = null;

			curChatIndex = 0;
			UIManager.Instance.SetOverlayUI(OverlayUI.None);
		}

		public void StartLine(LineData lineData)
		{
			if (lineLoop != null)
				StopCoroutine(lineLoop);

			var unit = DataManager.Instance.UnitDic[lineData.unitID];

			// TODO : 유닛 이미지 바리에이션 어떻게 저장하고 불러온 것인지?

			unitImage.sprite = unit.Thumbnail;
			unitName.text = unit.Name;
			lineText.text = string.Empty;

			this._lineData = lineData;

			lineLoop = StartCoroutine(LineLoop());
		}

		public IEnumerator LineLoop()
		{
			IsPrinting = true;

			foreach (var c in _lineData.line)
			{
				lineText.text += c;
				if (c != ' ')
					RuntimeManager.PlayOneShot("event:/SFX/Equip");
				yield return ws01;
			}

			EndLine();
		}

		public void SkipLine()
		{
			if (IsPrinting == false)
				return;

			if (lineLoop != null)
				StopCoroutine(lineLoop);

			lineText.text = _lineData.line;

			EndLine();
		}

		private void EndLine()
		{
			IsPrinting = false;
			if (_lineData.additionalData.Equals("0"))
			{
				// Debug.Log("Hawawaaaaaaaaaaaaaaaaaaaaaaaaa");
				// UIManager.Instance.OpenPanel(UIManager.MenuPanelType.Inventory);
			}
		}

		// =====================

		[SerializeField] private TextAsset chatScripts;
		private readonly Dictionary<string, List<LineData>> chatDataDic = new();

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