namespace WitchMendokusai.DomainSDK.Idle
{
	/// <summary>던전 4종 (economy.md 4). 무엇을 얻으러 가나로 나뉜다</summary>
	public enum IdleDungeonKind
	{
		/// <summary>재화 던전. 골드</summary>
		Gold = 0,

		/// <summary>보스 던전. 환생 조각과 장비</summary>
		Boss = 1,

		/// <summary>장비 던전. 장비 (부위 고정)</summary>
		Gear = 2,

		/// <summary>스킬 던전. 스킬 재료</summary>
		Skill = 3,
	}

	/// <summary>
	/// 던전 입장권 (economy.md 3, 4). 재화가 아니라 <b>하루 몇 번</b> 이라는 울타리.
	///
	/// ★ 재화로 만들면 모아 두었다 몰아 쓰는 것이 늘 정답이 되어 매일 들어올 이유가 사라짐.
	///   그래서 날이 바뀌면 <b>상한까지 채우고 끝</b>, 안 쓴 날치는 안 쌓임
	///
	/// ★ 날 경계는 <c>DayResetOffsetSeconds</c> 로 옮긴다. 자정에 끊으면 아직 노는 사람이 하루를
	///   두 번 겪는다. 수집형이 새벽에 끊는 이유 (기본값은 KST 05:00, 판정 대기)
	///
	/// ★ 판정에 실시각을 쓰는 유일한 층. 나머지는 전부 흐른 초로 돈다. 그래서 여기만
	///   <c>nowUnixSeconds</c> 를 받고, 오프라인 정산과 같은 자리에서 부른다 (IdleSession.CatchUp)
	/// </summary>
	public static class IdleDungeons
	{
		/// <summary>던전 수. 화면과 시험이 이 수로 돈다</summary>
		public const int COUNT = 4;

		private const long SECONDS_PER_DAY = 86400L;

		/// <summary>
		/// 그 시각이 속한 날 번호. 경계를 <c>offset</c> 만큼 뒤로 민 셈
		///
		/// ★ 음수 초(1970 이전)도 아래로 내림. C# 나눗셈은 0 쪽으로 자르므로 그대로 쓰면
		///   경계 하나가 두 배로 길어짐
		/// </summary>
		public static long DayIndexOf(long unixSeconds, long offsetSeconds)
		{
			long shifted = unixSeconds - offsetSeconds;
			long day = shifted / SECONDS_PER_DAY;

			if (shifted < 0L && shifted % SECONDS_PER_DAY != 0L)
			{
				day -= 1L;
			}

			return day;
		}

		/// <summary>
		/// 날이 바뀌었으면 입장권을 상한까지. 같은 날이면 아무 일도 없음
		///
		/// ★ 첫 판(마지막 채운 날이 없음)도 채움. 안 그러면 시작하자마자 하루를 기다려야 하는 판
		/// </summary>
		public static void Refill(IdleState state, IdleTuning tuning, long nowUnixSeconds)
		{
			state.EnsureTicketRoom();

			long today = DayIndexOf(nowUnixSeconds, tuning.DayResetOffsetSeconds);

			if (state.TicketDay == today)
			{
				return;
			}

			state.TicketDay = today;

			for (int index = 0; index < state.Tickets.Length; index++)
			{
				state.Tickets[index] = tuning.TicketsPerDay;
			}
		}

		/// <summary>남은 입장권</summary>
		public static long TicketsOf(IdleState state, IdleDungeonKind kind)
		{
			state.EnsureTicketRoom();

			int index = (int)kind;
			return index >= 0 && index < state.Tickets.Length ? state.Tickets[index] : 0L;
		}

		/// <summary>입장권 한 장을 쓴다. 없으면 아무 일도 안 일어난다</summary>
		public static bool TrySpend(IdleState state, IdleDungeonKind kind)
		{
			state.EnsureTicketRoom();

			int index = (int)kind;

			if (index < 0 || index >= state.Tickets.Length || state.Tickets[index] <= 0L)
			{
				return false;
			}

			state.Tickets[index] -= 1L;
			return true;
		}

		/// <summary>다음 채워지기까지 남은 초. 화면이 날짜 계산을 다시 하지 않게</summary>
		public static double SecondsUntilRefill(IdleState state, IdleTuning tuning, long nowUnixSeconds)
		{
			long today = DayIndexOf(nowUnixSeconds, tuning.DayResetOffsetSeconds);

			if (state.TicketDay != today)
			{
				return 0d;
			}

			long nextBoundary = (today + 1L) * SECONDS_PER_DAY + tuning.DayResetOffsetSeconds;
			return nextBoundary - nowUnixSeconds;
		}

		/// <summary>던전 규칙. 값은 SO 가 준 IdleTuning.Dungeons, 없으면 코드 기본값</summary>
		public static IdleDungeonSpec SpecOf(IdleTuning tuning, IdleDungeonKind kind)
		{
			IdleDungeonSpec[] specs = tuning.Dungeons;
			if (specs != null)
			{
				for (int index = 0; index < specs.Length; index++)
				{
					if (specs[index] != null && specs[index].Kind == kind)
					{
						return specs[index];
					}
				}
			}

			IdleDungeonSpec[] defaults = IdleDungeonSpec.Defaults();
			return defaults[(int)kind];
		}

		/// <summary>
		/// 그 던전이 지금 열려 있나. 스킬 던전은 스킬 재료가 아직 없어 닫혀 있음 (economy.md 표 2)
		///
		/// ★ 화면이 이유를 말하려면 여닫힘과 입장권을 따로 물어야 함. 입장권이 0 인 것과
		///   아직 안 만든 것은 사람에게 다른 말
		/// </summary>
		public static bool IsOpen(IdleTuning tuning, IdleDungeonKind kind)
		{
			return SpecOf(tuning, kind).Open;
		}

		/// <summary>한 번이라도 끝까지 깬 던전인가. 소탕이 열리는 조건 (사용자 2026-09-08)</summary>
		public static bool IsCleared(IdleState state, IdleDungeonKind kind)
		{
			return (state.DungeonCleared & (1L << (int)kind)) != 0L;
		}

		/// <summary>
		/// 한 판 입장 (changes/idle-dungeon-run). 입장권 한 장에 <b>판 시작</b>. 보상은 안에서 싸워 얻음
		///
		/// ★ 이미 판이 살아 있으면 안 됨. 던전 안에서 던전을 못 감
		/// ★ 시작하면 전장 다시 세움 (battle.Ready 를 내림). 다음 틱에 던전 웨이브 생김
		/// </summary>
		public static bool TryStart(IdleState state, IdleTuning tuning, IdleDungeonKind kind)
		{
			if (state.Dungeon.Active || IsOpen(tuning, kind) == false || TrySpend(state, kind) == false)
			{
				return false;
			}

			IdleDungeonSpec spec = SpecOf(tuning, kind);
			IdleDungeonRun run = state.Dungeon;
			run.Clear();
			run.Active = true;
			run.Kind = kind;
			run.TimeLimitSeconds = spec.TimeLimitSeconds;
			run.SecondsLeft = spec.TimeLimitSeconds;
			run.Waves = spec.Waves;

			IdleSquad.HealAll(state, tuning);
			state.Battle.Ready = false;
			return true;
		}

		/// <summary>시간이 흐름. 시간 제한이 있고 다 됐으면 끝 (재화 던전은 그때가 클리어). 반환은 끝났나</summary>
		public static bool TickRun(IdleState state, IdleTuning tuning, double delta)
		{
			IdleDungeonRun run = state.Dungeon;
			if (run.Active == false || run.TimeLimitSeconds <= 0d)
			{
				return false;
			}

			run.SecondsLeft -= delta;
			if (run.SecondsLeft > 0d)
			{
				return false;
			}

			run.SecondsLeft = 0d;
			EndRun(state, tuning, run.Kind == IdleDungeonKind.Gold);
			return true;
		}

		/// <summary>던전 안 처치 하나. 재화 던전은 처치마다 골드. 보스를 잡으면 조각과 장비를 주고 끝. 반환은 끝났나</summary>
		public static bool OnKill(IdleState state, IdleTuning tuning, bool boss)
		{
			IdleDungeonRun run = state.Dungeon;
			if (run.Active == false)
			{
				return false;
			}

			run.Kills += 1L;
			IdleDungeonSpec spec = SpecOf(tuning, run.Kind);

			if (run.Kind == IdleDungeonKind.Gold && spec.GoldSecondsPerKill > 0d)
			{
				double gold = IdleModel.IncomePerSecond(state, tuning) * spec.GoldSecondsPerKill;
				state.Resource += gold;
				run.Gold += gold;
			}

			if (run.Kind == IdleDungeonKind.Boss && boss)
			{
				run.Shards += spec.Shards > 0L ? spec.Shards : 0L;
				state.PrestigeShards += spec.Shards > 0L ? spec.Shards : 0L;
				run.Gear += IdleGear.Stow(state, tuning, GearTierOf(state, tuning), spec.GearCount);
				EndRun(state, tuning, true);
				return true;
			}

			return false;
		}

		/// <summary>던전 안 웨이브 하나를 다 잡음. 장비 던전은 웨이브마다 장비, 다 밀면 끝. 반환은 끝났나</summary>
		public static bool OnWaveCleared(IdleState state, IdleTuning tuning)
		{
			IdleDungeonRun run = state.Dungeon;
			if (run.Active == false)
			{
				return false;
			}

			run.WavesCleared += 1;

			if (run.Kind == IdleDungeonKind.Gear)
			{
				IdleDungeonSpec spec = SpecOf(tuning, run.Kind);
				run.Gear += IdleGear.Stow(state, tuning, GearTierOf(state, tuning), spec.GearCount);

				if (run.Waves > 0 && run.WavesCleared >= run.Waves)
				{
					EndRun(state, tuning, true);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 판 끝. 얻은 것은 이미 상태에 들어가 있음. 결과를 남기고 전장을 원래 구역으로 (사용자 2026-09-08: 나가면 원래 구역 + 결과 팝업)
		/// 깼으면 소탕이 열림. 전멸이나 시간 끝 (재화 제외) 은 그때까지 얻은 것만
		/// </summary>
		public static void EndRun(IdleState state, IdleTuning tuning, bool cleared)
		{
			IdleDungeonRun run = state.Dungeon;
			if (run.Active == false)
			{
				return;
			}

			double spent = run.TimeLimitSeconds > 0d ? run.TimeLimitSeconds - run.SecondsLeft : 0d;
			state.LastDungeonResult = new IdleDungeonResult(run.Kind, cleared, spent, run.Kills, run.WavesCleared, run.Gold, run.Shards, run.Gear);
			state.DungeonResultSequence += 1L;

			if (cleared)
			{
				state.DungeonCleared |= 1L << (int)run.Kind;
			}

			run.Clear();
			IdleSquad.HealAll(state, tuning);
			state.Battle.Ready = false;
		}

		/// <summary>던전이 주는 장비 등급. 지금 구역의 최고 등급</summary>
		public static int GearTierOf(IdleState state, IdleTuning tuning)
		{
			return IdleDrops.MaxTierAt(state.Stage, state.Ascensions, tuning);
		}

		/// <summary>소탕 한 판이 주는 골드 (재화 던전). 화면이 줄에 적을 때도 같은 셈</summary>
		public static double SweepGoldOf(IdleState state, IdleTuning tuning, IdleDungeonKind kind)
		{
			IdleDungeonSpec spec = SpecOf(tuning, kind);
			return kind == IdleDungeonKind.Gold ? IdleModel.IncomePerSecond(state, tuning) * spec.SweepGoldSeconds : 0d;
		}

		/// <summary>
		/// 남은 입장권을 한 번에 쓴다 (소탕). 한 번이라도 깬 던전만 (사용자 2026-09-08). 한 판은 풀 클리어 몫
		///
		/// ★ 무작위 없음. 사람이 누를 때만 도는 자리지만 보상까지 굴리면 저장을 껐다 켜서 다시 뽑는 길이 생김
		/// ★ 가방이 차면 장비는 그만 들어오지만 골드와 조각은 계속 들어옴
		/// </summary>
		public static bool TrySweep(IdleState state, IdleTuning tuning, IdleDungeonKind kind, out IdleDungeonReward reward)
		{
			reward = new IdleDungeonReward(kind, 0, 0d, 0L, 0);

			if (state.Dungeon.Active || IsOpen(tuning, kind) == false || IsCleared(state, kind) == false)
			{
				return false;
			}

			IdleDungeonSpec spec = SpecOf(tuning, kind);
			int runs = 0;
			double gold = 0d;
			long shards = 0L;
			int gear = 0;
			int tier = GearTierOf(state, tuning);

			while (TrySpend(state, kind))
			{
				runs++;
				switch (kind)
				{
					case IdleDungeonKind.Gold:
						double got = SweepGoldOf(state, tuning, kind);
						state.Resource += got;
						gold += got;
						break;
					case IdleDungeonKind.Boss:
						shards += spec.Shards > 0L ? spec.Shards : 0L;
						state.PrestigeShards += spec.Shards > 0L ? spec.Shards : 0L;
						gear += IdleGear.Stow(state, tuning, tier, spec.GearCount);
						break;
					case IdleDungeonKind.Gear:
						gear += IdleGear.Stow(state, tuning, tier, spec.GearCount * (spec.Waves > 0 ? spec.Waves : 1));
						break;
				}
			}

			if (runs == 0)
			{
				return false;
			}

			reward = new IdleDungeonReward(kind, runs, gold, shards, gear);
			return true;
		}
	}

	/// <summary>던전 한 번(또는 소탕 한 번)이 준 것. 화면이 그대로 적는다</summary>
	public readonly struct IdleDungeonReward
	{
		public IdleDungeonReward(IdleDungeonKind kind, int runs, double gold, long shards, int gear)
		{
			Kind = kind;
			Runs = runs;
			Gold = gold;
			Shards = shards;
			Gear = gear;
		}

		public IdleDungeonKind Kind { get; }

		/// <summary>몇 판을 돌았나. 소탕이면 한 번에 여러 판</summary>
		public int Runs { get; }

		public double Gold { get; }

		public long Shards { get; }

		/// <summary>가방에 실제로 들어간 장비 수. 가방이 차면 준 것보다 적다</summary>
		public int Gear { get; }
	}
}
