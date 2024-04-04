using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cinemachine;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIChat : MonoBehaviour
	{
		[SerializeField] private CinemachineTargetGroup chatTargetGroup;
		[SerializeField] private Image unitImage;
		[SerializeField] private TextMeshProUGUI unitName;
		[SerializeField] private TextMeshProUGUI lineText;
		[SerializeField] private CanvasGroup bubbleCanvasGroup;

		private int unitID;
		private Action endAction;

		public void StartChat(NPCObject npc, Action action)
		{
			if (TryGetChatData("테스트", out List<LineData> curChatDatas) == false)
				return;

			chatTargetGroup.m_Targets[1].target = npc.transform;
			endAction = action;

			SOManager.Instance.IsChatting.RuntimeValue = true;

			StartCoroutine(ChatLoop(curChatDatas));
		}

		private IEnumerator ChatLoop(List<LineData> curChatDatas)
		{
			StartCoroutine(BubbleLoop());
			bubbleCanvasGroup.alpha = 1;
			
			yield return null;
		
			foreach (LineData lineData in curChatDatas)
			{
				// HACK: 현재 플레이어가 플레이하고 있는 인형
				// TODO: 유닛 이미지 바리에이션 어떻게 저장하고 불러온 것인지?
				Unit unit = lineData.unitID == -1 ? GetDoll(0) : GetNPC(lineData.unitID);
				unitID = lineData.unitID;
				unitImage.sprite = unit.Sprite;
				unitName.text = unit.Name;

				Coroutine coroutine = StartCoroutine(PrintLine(lineData));

				do yield return null;
				while (lineText.text != lineData.line && Input.anyKeyDown == false);

				StopCoroutine(coroutine);
				lineText.text = lineData.line;

				do yield return null;
				while (Input.anyKeyDown == false);
			}

			SOManager.Instance.IsChatting.RuntimeValue = false;
			bubbleCanvasGroup.alpha = 0;
			StopAllCoroutines();

			endAction?.Invoke();
		}

		// 바로 null로 만드니 블렌드 전에 뚝 끊김
		// 어차피 새로 Chat 시작하면 target을 그 때 설정하니까
		// chatTargetGroup.m_Targets[1].target = null;
	
		private IEnumerator PrintLine(LineData lineData)
		{
			const float waitTime = 0.05f;
			WaitForSecondsRealtime wait = new(waitTime);
			StringBuilder s = new();

			s.Clear();
			foreach (char c in lineData.line)
			{
				s.Append(c);
				lineText.text = s.ToString();
				if (c != ' ')
					RuntimeManager.PlayOneShot("event:/SFX/Equip");
				yield return wait;
			}

			// EndLine
			// if (lineData.additionalData.Equals("0"))
		}

		public IEnumerator BubbleLoop()
		{
			const float BubblePadding = 30f;
			RectTransform bubbleRectTransform = bubbleCanvasGroup.GetComponent<RectTransform>();
			float bubbleWidth = bubbleRectTransform.sizeDelta.x;

			while (true)
			{
				// Update Bubble Pos
				Vector3 targetPos;
				if (unitID == 0)
					targetPos = chatTargetGroup.m_Targets[0].target.position;
				else
					targetPos = chatTargetGroup.m_Targets[1].target.position;
				bubbleCanvasGroup.transform.position = GetVec(targetPos + Vector3.up);

				yield return null;
			}

			Vector3 GetVec(Vector3 worldPos)
			{
				float bubbleHeight = bubbleRectTransform.sizeDelta.y;
				Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
				
				return new Vector3(
					Mathf.Clamp(screenPos.x, bubbleWidth / 2 + BubblePadding, Screen.width - bubbleWidth / 2 - BubblePadding),
					Mathf.Clamp(screenPos.y + 40, BubblePadding, Screen.height - bubbleHeight - BubblePadding), 0);
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
			string myString = chatScripts.text;

			// Debug.Log(myString);

			string csvText = myString[..(chatScripts.text.Length - 1)];
			string[] rows = csvText.Split(new[] { '\n' });

			string eventName = string.Empty;
			List<LineData> lineDatas = new();

			for (int i = 1; i < rows.Length; i++)
			{
				string[] columns = rows[i].Split(',');

				if (columns[0] == "end")
					continue;

				if (columns[0] != string.Empty)
				{
					eventName = columns[0];
					lineDatas = new List<LineData>();
				}

				LineData chatData = new(ref columns);
				lineDatas.Add(chatData);
			}
			chatDataDic.Add(eventName, lineDatas);
		}
	}

	public struct LineData
	{
		public int unitID;
		public string line;
		public string additionalData;

		public LineData(ref string[] columns)
		{
			unitID = int.Parse(columns[1]);
			line = columns[2];
			additionalData = columns[3].TrimEnd('\r', '\n', ' ');
		}
	}
}