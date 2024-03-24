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

		private CardManager cardManager;
		private MonsterSpawner monsterSpawner;
		private ExpManager expChecker;

		protected override void Awake()
		{
			base.Awake();

			cardManager = FindObjectOfType<CardManager>(true);
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
				UIManager.Instance.SetOverlay(MPanelType.None);
				UIManager.Instance.SetCanvas(MCanvasType.Dungeon);
				monsterSpawner.transform.position = PlayerController.Instance.transform.position;
				monsterSpawner.InitWaves(dungeon);

				expChecker.Init();
				cardManager.Init();
				PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

				foreach (Effect effect in DataManager.Instance.GetEquipment(DataManager.Instance.CurDollID, 0)?.Effects)
					effect.Apply();
				foreach (Effect effect in DataManager.Instance.GetEquipment(DataManager.Instance.CurDollID, 1)?.Effects)
					effect.Apply();
				foreach (Effect effect in DataManager.Instance.GetEquipment(DataManager.Instance.CurDollID, 2)?.Effects)
					effect.Apply();

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
			UIManager.Instance.SetOverlay(MPanelType.DungeonResult);
		}

		public void Continue()
		{
			// 집으로 돌아가기
			StageManager.Instance.LoadStage(StageManager.Instance.LastStage, -1, ResetDungeonAndPlayer);

			void ResetDungeonAndPlayer()
			{
				UIManager.Instance.SetOverlay(MPanelType.None);
				UIManager.Instance.SetCanvas(MCanvasType.None);
				GameManager.Instance.ClearDungeonObjects();

				expChecker.Init();
				cardManager.ClearCardEffect();
				PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

				for (int i = 0; i < 3; i++)
				{
					foreach (Effect effect in DataManager.Instance.GetEquipment(DataManager.Instance.CurDollID, i)!.Effects)
						effect.Apply();
				}
			}
		}

		private void UpdateTime()
		{
			DungeonCurTime -= TimeUpdateInterval;
		}

		private void UpdateDifficulty()
		{
			CurDifficulty = (Difficulty)((InitialDungeonTime - DungeonCurTime).TotalMinutes / 3);
		}
	}
}