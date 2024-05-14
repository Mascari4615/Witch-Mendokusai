using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public enum TextType
	{
		Normal = 0,
		Critical = 1,

		Heal = 100,

		Exp = 150,
	}

	public class UIFloatingText : MonoBehaviour
	{
		[SerializeField] private Transform textsRoot;
		[SerializeField] private GameObject textPrefab;
		private readonly Stack<(Animator animator, TextMeshProUGUI text)> texts = new();

		private void Start()
		{
			for (int i = 0; i < textsRoot.childCount; i++)
			{
				Animator animator = textsRoot.GetChild(i).GetComponent<Animator>();
				animator.keepAnimatorStateOnDisable = true;
				animator.gameObject.SetActive(false);
				texts.Push((animator, animator.transform.GetChild(0).GetComponent<TextMeshProUGUI>()));
			}
		}

		private (Animator, TextMeshProUGUI) Pop()
		{
			if (texts.Count == 0)
			{
				for (int i = 0; i < 10; i++)
				{
					GameObject newObject = Instantiate(textPrefab, textsRoot);
					Animator animator = newObject.GetComponent<Animator>();
					animator.keepAnimatorStateOnDisable = true;
					animator.gameObject.SetActive(false);
					texts.Push((animator, animator.transform.GetChild(0).GetComponent<TextMeshProUGUI>()));
				}
			}

			return texts.Pop();
		}

		public IEnumerator AniTextUI(Vector3 pos, TextType textType, string msg)
		{
			(Animator animator, TextMeshProUGUI text) = Pop();

			animator.SetTrigger("POP");
			animator.gameObject.SetActive(true);

			text.text = msg;
			UpdateTextStyle(ref text, textType);

			Vector3 lastPosition = pos + (Random.insideUnitSphere * .3f);
			float time = 0;
			while (true)
			{
				animator.transform.position = Camera.main.WorldToScreenPoint(lastPosition);
				yield return null;
				time += Time.deltaTime;

				// damageText.gameObject.SetActive(PlayerBody.Local.IsTargetInSight(lastPosition));

				if (time >= 1)
					break;
			}

			animator.gameObject.SetActive(false);
			texts.Push((animator, text));
		}

		private void UpdateTextStyle(ref TextMeshProUGUI text, TextType textType)
		{
			switch (textType)
			{
				case TextType.Normal:
					text.color = Color.white;
					break;
				case TextType.Critical:
					text.color = new Color(1, 110f / 255f, 86f / 255f);
					break;
				case TextType.Heal:
					break;
				case TextType.Exp:
					break;
				default:
					break;
			}
		}
	}
}