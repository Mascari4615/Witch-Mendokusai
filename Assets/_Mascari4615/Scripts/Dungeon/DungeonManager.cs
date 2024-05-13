using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static Mascari4615.SOHelper;

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

				CreateDungeonQuest();
			}
		}

		private IEnumerator DungeonLoop()
		{
			// Debug.Log(nameof(DungeonLoop));
			WaitForSeconds ws01 = new(.1f);

			// HACK:
			int dungeonClear = SOManager.Instance.Statistics[StatisticsType.DUNGEON_CLEAR];

			while (true)
			{
				UpdateTime();
				UpdateDifficulty();
				monsterSpawner.UpdateWaves();

				if (dungeonClear < SOManager.Instance.Statistics[StatisticsType.DUNGEON_CLEAR])
				{
					EndDungeon();
					yield break;
				}

				yield return ws01;
			}
		}

		private void UpdateTime()
		{
			DungeonCurTime -= TimeUpdateInterval;
			SOManager.Instance.Statistics[StatisticsType.DUNGEON_TIME] = (int)(InitialDungeonTime.TotalSeconds - DungeonCurTime.TotalSeconds);
		}

		private void UpdateDifficulty()
		{
			CurDifficulty = (DungeonDifficulty)((InitialDungeonTime - DungeonCurTime).TotalMinutes / 3);
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

		private void CreateDungeonQuest()
		{
			// TEST:
			QuestInfo questInfo = new()
			{
				Type = QuestType.Dungeon,
				GameEvents = new(),
				Criterias = new(),
				CompleteEffects = new()
				{
					// HACK:
					new EffectInfo()
					{
						Type = EffectType.Statistics,
						Data = GetStatisticsData((int)StatisticsType.DUNGEON_CLEAR),
						ArithmeticOperator = ArithmeticOperator.Add,
						Value = 1,
					}
				},
				RewardEffects = new(),
				Rewards = new(),

				WorkTime = 0,
				AutoWork = false,
				AutoComplete = true,
			};

			string questName = string.Empty;

			DungeonRecord curRecord = dungeonRecorder.GetResultRecord();
			DungeonType dungeonType = CurDungeon.Type;

			switch (dungeonType)
			{
				case DungeonType.TimeSurvival:
					questName = "시간 동안 생존";
					questInfo.Criterias = new()
					{
						new CriteriaInfo()
						{
							Type = CriteriaType.Statistics,
							Data = GetStatisticsData((int)StatisticsType.DUNGEON_TIME),
							ComparisonOperator = ComparisonOperator.GreaterThanOrEqualTo,
							Value = (int)InitialDungeonTime.TotalSeconds,
							JustOnce = true,
						}
					};
					break;
				case DungeonType.Domination:
					// TODO:
					break;
				case DungeonType.KillCount:
					questName = "몬스터 처치";
					questInfo.Criterias = new()
					{
						new CriteriaInfo()
						{
							Type = CriteriaType.Statistics,
							Data = GetStatisticsData((int)StatisticsType.MONSTER_KILL),
							ComparisonOperator = ComparisonOperator.GreaterThanOrEqualTo,
							Value = CurDungeon.ClearValue,
							JustOnce = true,
						}
					};
					break;
				case DungeonType.Boss:
					questName = "보스 처치";
					questInfo.Criterias = new()
					{
						new CriteriaInfo()
						{
							Type = CriteriaType.Statistics,
							Data = GetStatisticsData((int)StatisticsType.BOSS_KILL),
							ComparisonOperator = ComparisonOperator.GreaterThanOrEqualTo,
							Value = CurDungeon.ClearValue,
							JustOnce = true,
						}
					};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			RuntimeQuest runtimeQuest = new(questInfo, questName);
			DataManager.Instance.QuestManager.AddQuest(runtimeQuest);
		}
	}
}