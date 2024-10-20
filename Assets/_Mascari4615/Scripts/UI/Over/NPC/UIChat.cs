using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Cinemachine;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Mascari4615.SOHelper;
using DG.Tweening;

namespace Mascari4615
{
	public class UIChat : MonoBehaviour
	{
		[SerializeField] private CinemachineTargetGroup chatTargetGroup;
		[SerializeField] private Image unitImage;
		[SerializeField] private TextMeshProUGUI unitName;
		[SerializeField] private TextMeshProUGUI lineText;
		[SerializeField] private CanvasGroup chatCanvasGroup;
		[SerializeField] private CanvasGroup bubbleCanvasGroup;

		private NPCObject curNPC;
		private int unitID;
		private Action endAction;

		private void Start()
		{
			chatCanvasGroup.alpha = 0;
			bubbleCanvasGroup.alpha = 0;
		}

		public void StartChat(NPCObject npc, Action action)
		{
			// Debug.Log($"{nameof(StartChat)}: {npc.UnitData.ID}");

			// TODO: CSV가 아니라 스크립터블 오브젝트로 관리 가능하게
			if (TryGetChatData(npc.UnitData.ID.ToString(), out List<LineData> curChatDatas) == false)
				return;

			curNPC = npc;
			chatTargetGroup.Targets[1].Object = npc.transform;
			endAction = action;

			GameManager.Instance.IsChatting = true;

			StartCoroutine(ChatLoop(curChatDatas));
		}

		private IEnumerator ChatLoop(List<LineData> curChatDatas)
		{
			// Debug.Log($"{nameof(ChatLoop)}");

			StartCoroutine(BubbleLoop());
			
			chatCanvasGroup.DOFade(1, 0.2f);
			bubbleCanvasGroup.DOFade(1, 0.2f);

			unitImage.color = Color.clear;
			unitName.text = string.Empty;
			lineText.text = string.Empty;

			yield return null;

			unitImage.color = Color.white;

			foreach (LineData lineData in curChatDatas)
			{
				// Debug.Log($"{nameof(ChatLoop)}: {lineData.line}");

				// TODO: 유닛 이미지 바리에이션 어떻게 저장하고 불러온 것인지?
				Unit unit = null;

				if (lineData.unitID == 0)
					unit = Get<Doll>(DataManager.Instance.CurDollID);
				else if (lineData.unitID == -1)
					unit = curNPC.Data;

				unitID = lineData.unitID;
				unitImage.sprite = unit.Sprite;
				unitImage.transform.DOScaleY(.9f, .02f).OnComplete(() => unitImage.transform.DOScaleY(1, .02f));
				unitName.text = unit.Name;

				Coroutine coroutine = StartCoroutine(PrintLine(lineData));

				do yield return null;
				while (lineText.text != lineData.line && Input.anyKeyDown == false);

				StopCoroutine(coroutine);
				lineText.text = lineData.line;

				do yield return null;
				while (Input.anyKeyDown == false);
			}

			GameManager.Instance.IsChatting = false;

			chatCanvasGroup.DOFade(0, 0.2f);
			bubbleCanvasGroup.DOFade(0, 0.2f);

			StopAllCoroutines();

			endAction?.Invoke();
		}

		// 바로 null로 만드니 블렌드 전에 뚝 끊김
		// 어차피 새로 Chat 시작하면 target을 그 때 설정하니까
		// chatTargetGroup.m_Targets[1].target = null;

		private IEnumerator PrintLine(LineData lineData)
		{
			// Debug.Log($"{nameof(PrintLine)}: {lineData.line}");

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
			// Debug.Log($"{nameof(BubbleLoop)}");

			const float BubblePadding = 30f;
			RectTransform bubbleRectTransform = bubbleCanvasGroup.GetComponent<RectTransform>();
			float bubbleWidth = bubbleRectTransform.sizeDelta.x;

			while (true)
			{
				// Update Bubble Pos
				Vector3 targetPos;
				if (unitID == 0)
					targetPos = chatTargetGroup.Targets[0].Object.position;
				else
					targetPos = chatTargetGroup.Targets[1].Object.position;
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
				{
					chatDataDic.Add(eventName, lineDatas);
					eventName = string.Empty;
					lineDatas = new List<LineData>();
					continue;
				}

				if (columns[0] != string.Empty)
				{
					eventName = columns[0];
					lineDatas = new List<LineData>();
				}

				LineData chatData = new(ref columns);
				lineDatas.Add(chatData);
			}
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