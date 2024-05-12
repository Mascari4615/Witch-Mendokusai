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
			// TODO: 던전
			StageManager.Instance.LoadStage(dungeon.Stages[0], 0, InitDungeonAndPlayer);

			void InitDungeonAndPlayer()
			{
				UIManager.Instance.SetOverlay(MPanelType.None);
				UIManager.Instance.SetCanvas(MCanvasType.Dungeon);

				ObjectBufferManager.Instance.ClearObjects(ObjectType.Drop);
				ObjectBufferManager.Instance.ClearObjects(ObjectType.Monster);
				ObjectBufferManager.Instance.ClearObjects(ObjectType.Skill);
				ObjectBufferManager.Instance.ClearObjects(ObjectType.SpawnCircle);

				monsterSpawner.transform.position = Player.Instance.transform.position;
				monsterSpawner.InitWaves(dungeon);

				Player.Instance.Object.Init(GetDoll(DataManager.Instance.CurDollID));
				expChecker.Init();
				cardManager.Start_();

				List<EquipmentData> equipments = DataManager.Instance.GetEquipmentDatas(DataManager.Instance.CurDollID);
				foreach (EquipmentData equipment in equipments)
				{
					if (equipment == null)
						continue;

					Effect.ApplyEffects(equipment.Effects);

					if (equipment.Object != null)
						ObjectPoolManager.Instance.Spawn(equipment.Object).SetActive(true);
				}

				GameEventManager.Instance.Raise(GameEventType.OnDungeonStart);
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

				ObjectBufferManager.Instance.ClearObjects(ObjectType.Drop);
				ObjectBufferManager.Instance.ClearObjects(ObjectType.Monster);
				ObjectBufferManager.Instance.ClearObjects(ObjectType.Skill);
				ObjectBufferManager.Instance.ClearObjects(ObjectType.SpawnCircle);

				expChecker.Init();
				cardManager.ClearCardEffect();
				Player.Instance.Object.Init(Player.Instance.Object.UnitData);

				List<EquipmentData> equipments = DataManager.Instance.GetEquipmentDatas(DataManager.Instance.CurDollID);
				foreach (EquipmentData equipment in equipments)
				{
					if (equipment == null)
						continue;

					Effect.ApplyEffects(equipment.Effects);

					if (equipment.Object != null)
						ObjectPoolManager.Instance.Spawn(equipment.Object).SetActive(true);
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