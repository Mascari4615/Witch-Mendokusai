using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIChat : MonoBehaviour
	{
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private Image unitImage;
		[SerializeField] private TextMeshProUGUI unitName;
		[SerializeField] private TextMeshProUGUI lineText;

		private LineData _lineData;
		private Coroutine lineLoop;
		private WaitForSeconds ws01 = new WaitForSeconds(.1f);

		public bool IsPrinting { get; private set; } = false;

		public void SetActive(bool value)
		{
			canvasGroup.alpha = value ? 1 : 0;
			canvasGroup.blocksRaycasts = value;
			canvasGroup.interactable = value;
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
	}
}