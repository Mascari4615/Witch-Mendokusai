using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public enum Difficulty
	{
		Easy,
		Normal,
		Hard
	}

	public class DungeonManager : Singleton<DungeonManager>
	{
		public static readonly TimeSpan TimeUpdateInterval = new(0, 0, 0, 0, 100);
		private static readonly TimeSpan InitialDungeonTime = new(0, 0, 15, 0, 0);

		[field: Header("_" + nameof(DungeonManager))]
		[SerializeField] private MasteryManager masteryManager;
		[SerializeField] private MonsterSpawner monsterSpawner;

		[SerializeField] private UIDungeonEntrance uiDungeonEntrance;
		[SerializeField] private UIDungeonCanvas uiCombatCanvas;
		[SerializeField] private UIDamage uiDamage;
		[SerializeField] private UIStageResult uiStageResult;

		[SerializeField] private ExpManager expChecker;

		public TimeSpan DungeonCurTime { get; private set; }
		public Difficulty CurDifficulty { get; private set; }

		private void Start()
		{
			uiStageResult.SetActive(false);
		}

		public void OpenDungeonEntranceUI(List<Dungeon> dungeonDatas)
		{
			Debug.Log(nameof(OpenDungeonEntranceUI));
			uiDungeonEntrance.OpenCanvas(dungeonDatas);
		}

		public void PopDamage(Vector3 pos, int damge)
		{
			StartCoroutine(uiDamage.DamageTextUI(pos, damge));
		}

		public void CombatIntro()
		{
		}

		[ContextMenu(nameof(StartDungeon))]
		public void StartDungeon(Dungeon dungeon)
		{
			Debug.Log($"{nameof(StartDungeon)}");
			UIManager.Instance.Transition(() =>
			{
				StageManager.Instance.LoadStage(dungeon, 0);
				monsterSpawner.transform.position = PlayerController.Instance.transform.position;
				GameManager.Instance.SetPlayerState(PlayerState.Combat);

				monsterSpawner.InitWaves(dungeon);

				uiCombatCanvas.SetActive(true);
				uiStageResult.SetActive(false);

				expChecker.Init();
				masteryManager.Init();
				PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

				foreach (var effect in DataManager.Instance.CurStuff(0)!.Effects)
					effect.OnEquip();
				foreach (var effect in DataManager.Instance.CurStuff(1)!.Effects)
					effect.OnEquip();
				foreach (var effect in DataManager.Instance.CurStuff(2)!.Effects)
					effect.OnEquip();

				SOManager.Instance.OnDungeonStart.Raise();
				StartCoroutine(DungeonLoop(dungeon));
				DungeonCurTime = InitialDungeonTime;
			});
		}

		private IEnumerator DungeonLoop(Dungeon dungeon)
		{
			Debug.Log(nameof(DungeonLoop));
			WaitForSeconds ws01 = new(.1f);

			while (true)
			{
				UpdateTime();
				UpdateDifficulty();

				monsterSpawner.UpdateWaves();
				uiCombatCanvas.UpdateUI();

				yield return ws01;
			}
		}

		public void EndDungeon()
		{
			Debug.Log($"{nameof(EndDungeon)}");

			// Stop DungeonLoop
			StopAllCoroutines();
			monsterSpawner.StopWave();

			uiStageResult.Init();
			uiStageResult.SetActive(true);
			uiCombatCanvas.SetActive(false);
		}

		public void Continue()
		{
			// 집으로 돌아가기
			uiStageResult.SetActive(false);

			GameManager.Instance.ClearDungeonObjects();
			GameManager.Instance.SetPlayerState(PlayerState.Peaceful);

			StageManager.Instance.LoadStage(StageManager.Instance.LastStage, -1);

			masteryManager.ClearMasteryEffect();
			PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

			for (int i = 0; i < 3; i++)
			{
				foreach (var effect in DataManager.Instance.CurStuff(i)!.Effects)
					effect.OnEquip();
			}
		}

		private void UpdateTime()
		{
			DungeonCurTime -= TimeUpdateInterval;
		}

		private void UpdateDifficulty()
		{
			if (CurDifficulty == Difficulty.Hard)
			{

			}
			else if ((int)CurDifficulty != DungeonCurTime.Minutes / 3)
				CurDifficulty = (Difficulty)(DungeonCurTime.Minutes / 3);
		}
	}
}