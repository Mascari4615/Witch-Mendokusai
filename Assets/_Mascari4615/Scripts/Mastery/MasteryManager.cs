using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class MasteryManager : MonoBehaviour
	{
		[SerializeField] private MasteryDataBuffer masteryDataBuffer;
		[SerializeField] private MasteryDataBuffer selectMasteryDataBuffer;
		[SerializeField] private GameObject selectMasteryPanel;
		[SerializeField] private Image[] buttonImages;
		[SerializeField] private UISkillSlot[] skillSlots;
		[SerializeField] private TextMeshProUGUI[] texts;
		[SerializeField] private TextMeshProUGUI stackText;
		[SerializeField] private UIMasterySelectPanel[] masterySelectPanels;
		private int[] _choices = new int[3];

		// [SerializeField] private ToolTipTrigger[] toolTipTriggers;
		private int _selectMasteryStack;

		private int SelectMasteryStack
		{
			get => _selectMasteryStack;
			set
			{
				_selectMasteryStack = value;
				stackText.text = _selectMasteryStack > 1 ? $"x{_selectMasteryStack}" : string.Empty;
			}
		}

		private void Awake()
		{
			selectMasteryPanel.SetActive(false);
			SelectMasteryStack = 0;
		}

		private void Update()
		{
			UpdateCurMasteryUI();
		}

		private void UpdateCurMasteryUI()
		{
			int skillCount = 0;
			foreach ((Skill skill, SkillCoolTime skillCoolTime) in PlayerController.Instance.PlayerObject.UnitSkillHandler.SkillDic.Values)
			{
				skillSlots[skillCount].SetArtifact(skill);
				skillSlots[skillCount++].UpdateCooltime(skill, skillCoolTime);
			}

			for (int i = 0; i < skillSlots.Length; i++)
				skillSlots[i].gameObject.SetActive(i < skillCount);
		}

		public void Init()
		{
			while (selectMasteryDataBuffer.RuntimeItems.Count > 0)
				selectMasteryDataBuffer.RemoveItem(selectMasteryDataBuffer.RuntimeItems[^1]);
			selectMasteryDataBuffer.ClearBuffer();

			masteryDataBuffer.ClearBuffer();

			masteryDataBuffer.RuntimeItems.AddRange(DataManager.Instance.CurDoll.Masteries);
			masteryDataBuffer.RuntimeItems.AddRange(DataManager.Instance.CurStuff(0)!.Masteries);
			masteryDataBuffer.RuntimeItems.AddRange(DataManager.Instance.CurStuff(1)!.Masteries);
			masteryDataBuffer.RuntimeItems.AddRange(DataManager.Instance.CurStuff(2)!.Masteries);

			selectMasteryPanel.SetActive(false);
			SelectMasteryStack = 0;

			foreach (UIMasterySelectPanel masterySelectPanel in masterySelectPanels)
				masterySelectPanel.Init();
		}

		public void LevelUp()
		{
			if (masteryDataBuffer.RuntimeItems.Count < 3)
			{
				// Debug.Log("Not Enough Mastery Count");
				return;
			}

			SelectMasteryStack++;

			if (selectMasteryPanel.activeSelf == false)
				ShowMasterys();
		}

		public void ChooseAbility(int i)
		{
			TimeManager.Instance.Resume();
			RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
			// ToolTipManager.Instance.Hide();
			SelectMasteryStack--;

			Mastery randomMastery = masteryDataBuffer.RuntimeItems[_choices[i]];
			selectMasteryDataBuffer.AddItem(randomMastery);
			masteryDataBuffer.RuntimeItems.RemoveAt(_choices[i]);

			if (SelectMasteryStack > 0)
				ShowMasterys();
			else selectMasteryPanel.SetActive(false);
		}

		public void SelectMastery(Mastery mastery)
		{
			TimeManager.Instance.Resume();
			RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
			// ToolTipManager.Instance.Hide();
			SelectMasteryStack--;

			selectMasteryDataBuffer.AddItem(mastery);

			int index = masteryDataBuffer.RuntimeItems.IndexOf(mastery);
			masteryDataBuffer.RuntimeItems.RemoveAt(index);

			if (SelectMasteryStack > 0)
				ShowMasterys();
			else selectMasteryPanel.SetActive(false);
		}

		public void ClearMasteryEffect()
		{
			while (selectMasteryDataBuffer.RuntimeItems.Count > 0)
				selectMasteryDataBuffer.RemoveItem(selectMasteryDataBuffer.RuntimeItems[^1]);
		}

		public void ShowMasterys()
		{
			if (masteryDataBuffer.RuntimeItems.Count < 3)
			{
				Debug.Log("Not Enough Mastery Count");

				SelectMasteryStack = 0;
				selectMasteryPanel.SetActive(false);
				return;
			}
			TimeManager.Instance.Pause();
			selectMasteryPanel.SetActive(true);

			_choices = new int[] { -1, -1, -1 };
			List<Mastery> randomMasteries = new ();

			for (int i = 0; i < _choices.Length;)
			{
				int randomIndex = Random.Range(0, masteryDataBuffer.RuntimeItems.Count);

				if (_choices.Contains(randomIndex))
					continue;

				Mastery randomMastery = masteryDataBuffer.RuntimeItems[randomIndex];
				randomMasteries.Add(randomMastery);

				buttonImages[i].sprite = randomMastery.Thumbnail;
				texts[i].text = randomMastery.Name;

				// toolTipTriggers[i].SetToolTip(randomMasteries[i]);

				_choices[i] = randomIndex;
				i++;
			}

			// TODO : 해당되는 마스터리 UI에만 적용되도록 수정
			foreach (UIMasterySelectPanel masterySelectPanel in masterySelectPanels)
			{
				masterySelectPanel.UpdateUI(randomMasteries);
			}
		}
	}
}