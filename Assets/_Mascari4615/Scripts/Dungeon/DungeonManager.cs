using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class DungeonManager : Singleton<DungeonManager>
	{
		public static readonly TimeSpan TimeUpdateInterval = new(0, 0, 0, 0, 100);

		public Dungeon CurDungeon { get; private set; }
		public TimeSpan InitialDungeonTime { get; private set; } = new(0, 0, 15, 0, 0);
		public TimeSpan DungeonCurTime { get; private set; }
		public DungeonDifficulty CurDifficulty { get; private set; }
		public DungeonRecord Result { get; private set; }

		private CardManager cardManager;
		private MonsterSpawner monsterSpawner;
		private ExpManager expChecker;
		private DungeonRecorder dungeonRecorder;

		protected override void Awake()
		{
			base.Awake();

			cardManager = FindObjectOfType<CardManager>(true);
			monsterSpawner = FindObjectOfType<MonsterSpawner>(true);

			expChecker = FindObjectOfType<ExpManager>(true);
		}

		// TODO: 던전 인트로
		public void CombatIntro()
		{
		}

		public void StartDungeon(Dungeon dungeon)
		{
			Debug.Log($"{nameof(StartDungeon)}");

			CurDungeon = dungeon;

			// TODO: 던전
			StageManager.Instance.LoadStage(dungeon.Stages[0], 0, InitDungeonAndPlayer);

			void InitDungeonAndPlayer()
			{
				UIManager.Instance.SetOverlay(MPanelType.None);
				UIManager.Instance.SetCanvas(MCanvasType.Dungeon);

				monsterSpawner.transform.position = Player.Instance.transform.position;
				monsterSpawner.InitWaves(dungeon);

				GameManager.Instance.Init();
				expChecker.Init();
				cardManager.Init();

				InitialDungeonTime = new TimeSpan(0, dungeon.TimeByMinute, 0);
				DungeonCurTime = InitialDungeonTime;

				dungeonRecorder = new DungeonRecorder();

				StartCoroutine(DungeonLoop());
				GameEventManager.Instance.Raise(GameEventType.OnDungeonStart);
			}
		}

		private IEnumerator DungeonLoop()
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

		private void UpdateTime()
		{
			DungeonCurTime -= TimeUpdateInterval;
		}

		private void UpdateDifficulty()
		{
			CurDifficulty = (DungeonDifficulty)((InitialDungeonTime - DungeonCurTime).TotalMinutes / 3);
		}

		private void CheckClear()
		{
			DungeonType dungeonType = CurDungeon.Type;

			switch (dungeonType)
			{
				case DungeonType.TimeSurvival:
					if (DungeonCurTime <= TimeSpan.Zero)
						EndDungeon();
					break;
				case DungeonType.Domination:
					break;
				case DungeonType.KillCount:
					break;
				case DungeonType.Boss:

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void EndDungeon()
		{
			Debug.Log($"{nameof(EndDungeon)}");

			// Stop DungeonLoop
			StopAllCoroutines();
			monsterSpawner.StopWave();

			Result = dungeonRecorder.GetResultRecord();

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

				GameManager.Instance.Init();
				expChecker.Init();
				cardManager.ClearCardEffect();
			}
		}
	}
}