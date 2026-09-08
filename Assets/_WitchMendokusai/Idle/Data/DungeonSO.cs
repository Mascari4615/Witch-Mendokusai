using System;
using System.Collections.Generic;
using UnityEngine;
using WitchMendokusai.DomainSDK.Idle;

namespace WitchMendokusai.Idle
{
	/// <summary>
	/// 던전 하나의 규칙 (changes/idle-dungeon-run v2). 종류가 규칙, 난이도 3 x 스테이지 5 칸이 세기와 보상 (사용자 2026-09-08: 하드코딩 금지)
	///
	/// ★ 칸의 세기는 본판 구역 환산 (Level). 본판 곡선을 빌려 절대값을 정함. 그래야 표를 손으로 채울 수 있음
	/// </summary>
	[CreateAssetMenu(fileName = "IdleDungeon", menuName = "WM/Idle/Dungeon")]
	public sealed class DungeonSO : ScriptableObject
	{
		[Serializable]
		public sealed class StageData
		{
			[Tooltip("적 세기의 본판 구역 환산. 체력과 초당 피해 곡선을 이 구역 것으로")]
			public int level = 1;

			public double healthMultiplier = 1d;

			public double damageMultiplier = 1d;

			[Tooltip("시간 제한 (초). 0 이면 없음. 끝나면 판이 끝남 (재화는 그때가 클리어)")]
			public double timeLimitSeconds;

			[Tooltip("장비 던전의 웨이브 수. 다 밀면 클리어. 0 이면 웨이브 조건 없음")]
			public int waves;

			[Tooltip("한 웨이브 잡몹 수. 0 이면 본판 WaveSize")]
			public int waveSize;

			[Tooltip("재화 던전. 처치 하나가 주는 골드 (지금 초당 수입 x 이 초)")]
			public double goldSecondsPerKill;

			[Tooltip("재화 던전 소탕 한 판이 주는 골드 (지금 초당 수입 x 이 초)")]
			public double sweepGoldSeconds;

			[Tooltip("보스 던전. 보스를 잡으면 주는 환생 조각")]
			public long shards;

			[Tooltip("보스 던전은 잡았을 때, 장비 던전은 웨이브마다 주는 장비 수")]
			public long gearCount;

			[Tooltip("주는 장비 등급. 0 이면 Level 구역의 최고 등급")]
			public int gearTier;

			public IdleDungeonStageSpec ToDomain()
			{
				return new IdleDungeonStageSpec
				{
					Level = level,
					HealthMultiplier = healthMultiplier,
					DamageMultiplier = damageMultiplier,
					TimeLimitSeconds = timeLimitSeconds,
					Waves = waves,
					WaveSize = waveSize,
					GoldSecondsPerKill = goldSecondsPerKill,
					SweepGoldSeconds = sweepGoldSeconds,
					Shards = shards,
					GearCount = gearCount,
					GearTier = gearTier,
				};
			}
		}

		[Serializable]
		public sealed class TierData
		{
			public List<StageData> stages = new List<StageData>();
		}

		[SerializeField] private IdleDungeonKind kind = IdleDungeonKind.Gold;

		[Tooltip("닫힌 던전은 입장도 소탕도 못 함")]
		[SerializeField] private bool open = true;

		[Tooltip("난이도 순 (보통, 어려움, 지옥). 난이도마다 스테이지 5")]
		[SerializeField] private List<TierData> tiers = new List<TierData>();

		[Header("그림. 던전마다 개성 (사용자 2026-09-08). prefab 을 비우면 도형과 색")]
		[Tooltip("바닥 색. 난이도가 오르면 BattlePresentationSO.DungeonDifficultyShade 만큼 어두워짐")]
		[SerializeField] private Color floorColor = new Color(0.22f, 0.2f, 0.26f);

		[Tooltip("소품 도형")]
		[SerializeField] private Geometry.Shape sceneryShape = Geometry.Shape.Cube;

		[Tooltip("잡몹 도형. 보스는 다음 도형")]
		[SerializeField] private Geometry.Shape foeShape = Geometry.Shape.Cube;

		[Tooltip("잡몹 prefab. 비우면 도형")]
		[SerializeField] private GameObject foePrefab;

		[Tooltip("보스 prefab. 비우면 도형")]
		[SerializeField] private GameObject bossPrefab;
		[SerializeField] private List<GameObject> sceneryPrefabs = new List<GameObject>();

		public IReadOnlyList<GameObject> SceneryPrefabs => sceneryPrefabs;

		public IdleDungeonKind Kind => kind;

		public Color FloorColor => floorColor;

		public Geometry.Shape SceneryShape => sceneryShape;

		public Geometry.Shape FoeShape => foeShape;

		public GameObject FoePrefab => foePrefab;

		public GameObject BossPrefab => bossPrefab;

		public IdleDungeonSpec ToDomain()
		{
			IdleDungeonTier[] made = new IdleDungeonTier[tiers.Count];
			for (int difficulty = 0; difficulty < tiers.Count; difficulty++)
			{
				List<StageData> stages = tiers[difficulty].stages;
				IdleDungeonStageSpec[] cells = new IdleDungeonStageSpec[stages.Count];
				for (int stage = 0; stage < stages.Count; stage++)
				{
					cells[stage] = stages[stage].ToDomain();
				}

				made[difficulty] = new IdleDungeonTier { Stages = cells };
			}

			return new IdleDungeonSpec { Kind = kind, Open = open, Tiers = made };
		}
	}
}
