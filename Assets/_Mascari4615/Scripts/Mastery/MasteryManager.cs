using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
		private readonly List<List<Mastery>> masteryDataBuffers = new(3) {new(), new(), new()};
		[SerializeField] private MasteryDataBuffer selectMasteryDataBuffer;

		[SerializeField] private UISkillSlot[] skillSlots;
		[SerializeField] private TextMeshProUGUI stackText;

		[SerializeField] private GameObject selectDeckPanel;
		[SerializeField] private GameObject deckPanel;
		[SerializeField] private UICardSlot[] selectButtons;
		private readonly Dictionary<int, UIDeck> deckUIDic = new();

		private int _levelUpStack;
		private int LevelUpStack
		{
			get => _levelUpStack;
			set
			{
				_levelUpStack = value;
				stackText.text = _levelUpStack > 1 ? $"x{_levelUpStack}" : string.Empty;
			}
		}

		private void Awake()
		{
			selectDeckPanel.SetActive(false);
			deckPanel.SetActive(false);
			foreach (UIDeck deckUI in deckUIDic.Values)
				deckUI.gameObject.SetActive(false);
			LevelUpStack = 0;

			UIDeck[] deckUIs = FindObjectsOfType<UIDeck>(true);
			foreach (UIDeck deckUI in deckUIs)
				deckUIDic.Add(deckUI.EquipmentData.ID, deckUI);
		}

		public void Init()
		{
			selectDeckPanel.SetActive(false);
			deckPanel.SetActive(false);
			foreach (UIDeck deckUI in deckUIDic.Values)
				deckUI.gameObject.SetActive(false);

			while (selectMasteryDataBuffer.RuntimeItems.Count > 0)
				selectMasteryDataBuffer.RemoveItem(selectMasteryDataBuffer.RuntimeItems[^1]);
			selectMasteryDataBuffer.ClearBuffer();

			foreach (List<Mastery> masteryDataBuffer in masteryDataBuffers)
				masteryDataBuffer.Clear();

			for (int i = 0; i < 3; i++)
			{
				EquipmentData equipment = DataManager.Instance.GetEquipment(i);

				if (equipment == null)
					continue;
				masteryDataBuffers[i].AddRange(equipment.Masteries);

				deckUIDic[equipment.ID].Init(
					cardSelectAction: (Artifact a) =>
					{
						// SelectMastery(masterySlots[i].Atrifact as Mastery);
						// 원래 위 코드를 썼는데, 클로저 문제로 인해 아래처럼 바꿈

						SelectCard(a as Mastery);
					});

				selectButtons[i].SetSelectAction(
					(Artifact a) =>
					{
						SelectCard(a as Mastery);
					});
			}

			LevelUpStack = 0;
		}

		public void LevelUp()
		{
			// if (masteryDataBuffer.RuntimeItems.Count < 3)
			{
				// Debug.Log("Not Enough Mastery Count");
				// return;
			}

			LevelUpStack++;
			selectDeckPanel.SetActive(true);
			TimeManager.Instance.Pause();
		}

		public void ShowDeck(int index)
		{
			curIndex = index;
			List<Mastery> masteryDataBuffer = masteryDataBuffers[index];
		
			deckPanel.SetActive(true);
			selectDeckPanel.SetActive(false);
			
			if (masteryDataBuffer.Count < 3)
			{
				// Debug.Log("Not Enough Mastery Count");

				LevelUpStack--;
				return;
			}

			TimeManager.Instance.Pause();

			List<Mastery> randomMasteries = new();

			while (randomMasteries.Count < 3)
			{
				int randomIndex = Random.Range(0, masteryDataBuffer.Count);
				Mastery randomMastery = masteryDataBuffer[randomIndex];

				if (randomMasteries.Contains(randomMastery))
					continue;

				selectButtons[randomMasteries.Count].SetArtifact(randomMastery);
				randomMasteries.Add(randomMastery);

				// buttonImages[i].sprite = randomMastery.Thumbnail;
				// texts[i].text = randomMastery.Name;
			}

			// TODO : 해당되는 마스터리 UI에만 적용되도록 수정
			int equipmentID = DataManager.Instance.GetEquipment(index).ID;
			deckUIDic[equipmentID].UpdateUI(randomMasteries);
			
			deckUIDic[equipmentID].gameObject.SetActive(true);
		}
		int curIndex = 0;

		public void SelectCard(Mastery mastery)
		{
			TimeManager.Instance.Resume();
			RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
			// ToolTipManager.Instance.Hide();
			LevelUpStack--;

			selectMasteryDataBuffer.AddItem(mastery);

			if (mastery.MaxStack == selectMasteryDataBuffer.RuntimeItems.Select(m => m == mastery).Count())
			{
				int index = masteryDataBuffers[curIndex].IndexOf(mastery);
				masteryDataBuffers[curIndex].RemoveAt(index);
			}

			if (LevelUpStack > 0)
			{
				// ShowDeck();
			}
			else selectDeckPanel.SetActive(false);
			deckPanel.SetActive(false);
			foreach (UIDeck deckUI in deckUIDic.Values)
				deckUI.gameObject.SetActive(false);
		}

		public void ClearMasteryEffect()
		{
			while (selectMasteryDataBuffer.RuntimeItems.Count > 0)
				selectMasteryDataBuffer.RemoveItem(selectMasteryDataBuffer.RuntimeItems[^1]);
		}

		// ========

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
	}
}