using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
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

		public TimeSpan DungeonCurTime { get; private set; }
		public Difficulty CurDifficulty { get; private set; }

		private MasteryManager masteryManager;
		private MonsterSpawner monsterSpawner;
		private ExpManager expChecker;

		protected override void Awake()
		{
			base.Awake();

			masteryManager = FindObjectOfType<MasteryManager>(true);
			monsterSpawner = FindObjectOfType<MonsterSpawner>(true);

			expChecker = FindObjectOfType<ExpManager>(true);
		}

		public void CombatIntro()
		{
		}

		[ContextMenu(nameof(StartDungeon))]
		public void StartDungeon(Dungeon dungeon)
		{
			Debug.Log($"{nameof(StartDungeon)}");
			StageManager.Instance.LoadStage(dungeon, 0, InitDungeonAndPlayer);

			void InitDungeonAndPlayer()
			{
				UIManager.Instance.SetOverlayUI(OverlayUI.None);
				GameManager.Instance.SetContent(GameContent.Dungeon);
				monsterSpawner.transform.position = PlayerController.Instance.transform.position;
				monsterSpawner.InitWaves(dungeon);

				expChecker.Init();
				masteryManager.Init();
				PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

				foreach (var effect in DataManager.Instance.GetEquipment(0)!.Effects)
					effect.OnEquip();
				foreach (var effect in DataManager.Instance.GetEquipment(1)!.Effects)
					effect.OnEquip();
				foreach (var effect in DataManager.Instance.GetEquipment(2)!.Effects)
					effect.OnEquip();

				SOManager.Instance.OnDungeonStart.Raise();
				StartCoroutine(DungeonLoop(dungeon));
				DungeonCurTime = InitialDungeonTime;
			}
		}

		private IEnumerator DungeonLoop(Dungeon dungeon)
		{
			// Debug.Log(nameof(DungeonLoop));
			WaitForSeconds ws01 = new(.1f);

			while (true)
			{
				UpdateTime();
				UpdateDifficulty();

				monsterSpawner.UpdateWaves();

				yield return ws01;
			}
		}

		public void EndDungeon()
		{
			Debug.Log($"{nameof(EndDungeon)}");

			// Stop DungeonLoop
			StopAllCoroutines();
			monsterSpawner.StopWave();
			UIManager.Instance.SetOverlayUI(OverlayUI.DungeonResult);
		}

		public void Continue()
		{
			// 집으로 돌아가기
			StageManager.Instance.LoadStage(StageManager.Instance.LastStage, -1, ResetDungeonAndPlayer);

			void ResetDungeonAndPlayer()
			{
				UIManager.Instance.SetOverlayUI(OverlayUI.None);
				GameManager.Instance.SetContent(GameContent.None);
				GameManager.Instance.ClearDungeonObjects();

				masteryManager.ClearMasteryEffect();
				PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

				for (int i = 0; i < 3; i++)
				{
					foreach (var effect in DataManager.Instance.GetEquipment(i)!.Effects)
						effect.OnEquip();
				}
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