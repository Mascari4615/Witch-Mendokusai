using UnityEngine;
using WitchMendokusai.DomainSDK.Idle;

namespace WitchMendokusai.Idle
{
	/// <summary>
	/// 던전 하나의 판 규칙 (changes/idle-dungeon-run). 시간, 웨이브, 보상을 인스펙터에서 (사용자 2026-09-08: 하드코딩 금지)
	///
	/// ★ 종류마다 쓰는 칸이 다름. 재화는 시간과 처치당 골드, 보스는 시간과 조각과 장비, 장비는 웨이브와 웨이브당 장비
	/// </summary>
	[CreateAssetMenu(fileName = "IdleDungeon", menuName = "WM/Idle/Dungeon")]
	public sealed class DungeonSO : ScriptableObject
	{
		[SerializeField] private IdleDungeonKind kind = IdleDungeonKind.Gold;

		[Tooltip("닫힌 던전은 입장도 소탕도 못 함")]
		[SerializeField] private bool open = true;

		[Tooltip("시간 제한 (초). 0 이면 없음. 끝나면 판이 끝남 (재화는 그때가 클리어)")]
		[SerializeField] private double timeLimitSeconds = 90d;

		[Tooltip("장비 던전의 웨이브 수. 다 밀면 클리어. 0 이면 웨이브 조건 없음")]
		[SerializeField] private int waves;

		[Tooltip("재화 던전. 처치 하나가 주는 골드 (지금 초당 수입 x 이 초)")]
		[SerializeField] private double goldSecondsPerKill;

		[Tooltip("재화 던전 소탕 한 판이 주는 골드 (지금 초당 수입 x 이 초)")]
		[SerializeField] private double sweepGoldSeconds;

		[Tooltip("보스 던전. 보스를 잡으면 주는 환생 조각")]
		[SerializeField] private long shards;

		[Tooltip("보스 던전은 잡았을 때, 장비 던전은 웨이브마다 주는 장비 수")]
		[SerializeField] private long gearCount;

		public IdleDungeonKind Kind => kind;

		public IdleDungeonSpec ToDomain()
		{
			return new IdleDungeonSpec
			{
				Kind = kind,
				Open = open,
				TimeLimitSeconds = timeLimitSeconds,
				Waves = waves,
				GoldSecondsPerKill = goldSecondsPerKill,
				SweepGoldSeconds = sweepGoldSeconds,
				Shards = shards,
				GearCount = gearCount,
			};
		}
	}
}
