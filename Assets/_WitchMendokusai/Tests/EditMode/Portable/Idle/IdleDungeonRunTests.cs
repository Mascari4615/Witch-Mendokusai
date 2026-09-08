using NUnit.Framework;
using WitchMendokusai.DomainSDK.Idle;

namespace WitchMendokusai.Tests
{
	/// <summary>
	/// 던전 안 판 (changes/idle-dungeon-run, economy.md 표 2)
	///
	/// ★ 지키는 것: 입장권 한 장에 판 하나, 보상은 안에서 싸워 얻음, 시간과 보스와 웨이브가 끝을 정함,
	///   전멸은 얻은 것만 들고 나감 (구역 후퇴 없음), 소탕은 한 번 깬 던전만, 규칙 값은 튜닝이 줌
	/// </summary>
	public sealed class IdleDungeonRunTests
	{
		private static IdleState Ready(IdleTuning tuning)
		{
			IdleState state = new IdleState();
			IdleHeroes.EnsureStarter(state);
			state.EnsureSeatRoom(tuning);
			IdleDungeons.Refill(state, tuning, 0L);
			return state;
		}

		[Test]
		public void Enter_SpendsOneTicket_AndStartsTheRun()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			long before = IdleDungeons.TicketsOf(state, IdleDungeonKind.Gold);
			IdleDungeonSpec spec = IdleDungeons.SpecOf(tuning, IdleDungeonKind.Gold);

			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));

			Assert.IsTrue(state.Dungeon.Active);
			Assert.AreEqual(IdleDungeonKind.Gold, state.Dungeon.Kind);
			Assert.AreEqual(spec.TimeLimitSeconds, state.Dungeon.SecondsLeft, 1e-9d);
			Assert.AreEqual(before - 1L, IdleDungeons.TicketsOf(state, IdleDungeonKind.Gold));
			Assert.IsFalse(state.Battle.Ready, "판이 시작됐는데 전장을 다시 세우지 않는다");
		}

		/// <summary>★ 던전 안에서 던전을 못 감. 입장권도 안 씀</summary>
		[Test]
		public void Enter_WhileRunning_IsRefused()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));
			long before = IdleDungeons.TicketsOf(state, IdleDungeonKind.Boss);

			Assert.IsFalse(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Boss));
			Assert.AreEqual(before, IdleDungeons.TicketsOf(state, IdleDungeonKind.Boss));
			Assert.AreEqual(IdleDungeonKind.Gold, state.Dungeon.Kind);
		}

		[Test]
		public void GoldDungeon_PaysPerKill_AndClearsWhenTimeRunsOut()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			IdleDungeonSpec spec = IdleDungeons.SpecOf(tuning, IdleDungeonKind.Gold);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));
			double perKill = IdleModel.IncomePerSecond(state, tuning) * spec.GoldSecondsPerKill;

			Assert.IsFalse(IdleDungeons.OnKill(state, tuning, false), "처치 하나로 재화 던전이 끝났다");
			Assert.IsFalse(IdleDungeons.OnKill(state, tuning, false));
			Assert.AreEqual(2d * perKill, state.Resource, 1e-6d);
			Assert.AreEqual(2d * perKill, state.Dungeon.Gold, 1e-6d);

			Assert.IsFalse(IdleDungeons.TickRun(state, tuning, spec.TimeLimitSeconds * 0.5d));
			Assert.IsTrue(IdleDungeons.TickRun(state, tuning, spec.TimeLimitSeconds * 0.5d), "시간이 다 됐는데 안 끝났다");

			Assert.IsFalse(state.Dungeon.Active);
			Assert.IsTrue(state.LastDungeonResult.Cleared, "재화 던전은 시간을 버티면 클리어");
			Assert.AreEqual(2L, state.LastDungeonResult.Kills);
			Assert.AreEqual(2d * perKill, state.LastDungeonResult.Gold, 1e-6d);
			Assert.AreEqual(spec.TimeLimitSeconds, state.LastDungeonResult.SecondsSpent, 1e-6d);
			Assert.AreEqual(1L, state.DungeonResultSequence);
			Assert.IsTrue(IdleDungeons.IsCleared(state, IdleDungeonKind.Gold));
		}

		[Test]
		public void BossDungeon_KillingTheBoss_EndsWithShardsAndGear()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			IdleDungeonSpec spec = IdleDungeons.SpecOf(tuning, IdleDungeonKind.Boss);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Boss));

			Assert.IsTrue(IdleDungeons.OnKill(state, tuning, true), "보스를 잡았는데 안 끝났다");

			Assert.AreEqual(spec.Shards, state.PrestigeShards);
			Assert.AreEqual(spec.GearCount, (long)state.Bag.Count);
			Assert.IsTrue(state.LastDungeonResult.Cleared);
			Assert.AreEqual(spec.Shards, state.LastDungeonResult.Shards);
			Assert.AreEqual((int)spec.GearCount, state.LastDungeonResult.Gear);
			Assert.AreEqual(0d, state.LastDungeonResult.Gold, 1e-9d, "보스 던전이 골드를 줬다");
			Assert.IsTrue(IdleDungeons.IsCleared(state, IdleDungeonKind.Boss));
		}

		/// <summary>★ 보스를 못 잡고 시간이 끝나면 실패. 조각 없음, 소탕 안 열림</summary>
		[Test]
		public void BossDungeon_TimeOut_IsNotAClear()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			IdleDungeonSpec spec = IdleDungeons.SpecOf(tuning, IdleDungeonKind.Boss);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Boss));

			Assert.IsTrue(IdleDungeons.TickRun(state, tuning, spec.TimeLimitSeconds + 1d));

			Assert.IsFalse(state.LastDungeonResult.Cleared);
			Assert.AreEqual(0L, state.PrestigeShards);
			Assert.IsFalse(IdleDungeons.IsCleared(state, IdleDungeonKind.Boss));
		}

		[Test]
		public void GearDungeon_GivesGearPerWave_AndClearsAtTheLastWave()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			IdleDungeonSpec spec = IdleDungeons.SpecOf(tuning, IdleDungeonKind.Gear);
			Assert.Greater(spec.Waves, 1, "장비 던전 웨이브가 하나뿐이면 시험이 뜻이 없다");
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gear));

			for (int wave = 1; wave < spec.Waves; wave++)
			{
				Assert.IsFalse(IdleDungeons.OnWaveCleared(state, tuning), "마지막 웨이브 전에 끝났다 (" + wave + ")");
				Assert.AreEqual(spec.GearCount * wave, (long)state.Bag.Count);
			}

			Assert.IsTrue(IdleDungeons.OnWaveCleared(state, tuning), "마지막 웨이브를 밀었는데 안 끝났다");
			Assert.AreEqual(spec.GearCount * spec.Waves, (long)state.Bag.Count);
			Assert.IsTrue(state.LastDungeonResult.Cleared);
			Assert.AreEqual(spec.Waves, state.LastDungeonResult.WavesCleared);
		}

		[Test]
		public void GearDungeon_FillsTheBag_ButNotPastIt()
		{
			IdleTuning tuning = new IdleTuning();
			tuning.Dungeons = IdleDungeonSpec.Defaults();
			tuning.Dungeons[(int)IdleDungeonKind.Gear].GearCount = 500L;
			IdleState state = Ready(tuning);
			int room = IdleShop.BagCapacityOf(state, tuning);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gear));

			IdleDungeons.OnWaveCleared(state, tuning);

			Assert.AreEqual(room, state.Bag.Count, "가방보다 많이 들어갔다");
			Assert.AreEqual(room, state.Dungeon.Gear, "결과의 장비 수는 실제로 들어간 수");
		}

		/// <summary>★ 스킬 재료가 아직 없어 스킬 던전은 닫혀 있다. 입장권도 안 쓴다</summary>
		[Test]
		public void SkillDungeon_IsClosed()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			long before = IdleDungeons.TicketsOf(state, IdleDungeonKind.Skill);

			Assert.IsFalse(IdleDungeons.IsOpen(tuning, IdleDungeonKind.Skill));
			Assert.IsFalse(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Skill));
			Assert.AreEqual(before, IdleDungeons.TicketsOf(state, IdleDungeonKind.Skill));
		}

		/// <summary>★ 규칙 값은 튜닝 (SO) 몫. 비어 있으면 코드 기본값</summary>
		[Test]
		public void Specs_ComeFromTuning_OrFallBackToDefaults()
		{
			IdleTuning tuning = new IdleTuning();
			tuning.Dungeons = new[] { new IdleDungeonSpec { Kind = IdleDungeonKind.Gold, TimeLimitSeconds = 7d } };
			IdleState state = Ready(tuning);

			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));
			Assert.AreEqual(7d, state.Dungeon.SecondsLeft, 1e-9d);

			IdleDungeonSpec fallback = IdleDungeons.SpecOf(tuning, IdleDungeonKind.Boss);
			Assert.AreEqual(IdleDungeonSpec.Defaults()[(int)IdleDungeonKind.Boss].Shards, fallback.Shards);

			tuning.Dungeons = null;
			Assert.AreEqual(IdleDungeonSpec.Defaults()[(int)IdleDungeonKind.Gold].TimeLimitSeconds,
				IdleDungeons.SpecOf(tuning, IdleDungeonKind.Gold).TimeLimitSeconds, 1e-9d);
		}

		/// <summary>★ 시뮬 안: 보스 던전은 보스 하나, 재화 던전은 잡몹 무리. 구역 진행도와 무관</summary>
		[Test]
		public void Sim_DungeonWaves_IgnoreStageProgress()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState boss = Ready(tuning);
			boss.KillsInStage = 0;
			Assert.IsTrue(IdleDungeons.TryStart(boss, tuning, IdleDungeonKind.Boss));
			IdleBattleSim.Advance(boss, tuning, 0.1d);
			Assert.AreEqual(1, boss.Battle.Foes.Count);
			Assert.IsTrue(boss.Battle.Foes[0].Boss, "보스 던전 첫 웨이브가 보스가 아니다");

			IdleState gold = Ready(tuning);
			gold.KillsInStage = tuning.KillsPerStage - 1;
			Assert.IsTrue(IdleDungeons.TryStart(gold, tuning, IdleDungeonKind.Gold));
			IdleBattleSim.Advance(gold, tuning, 0.1d);
			Assert.AreEqual(tuning.WaveSize, gold.Battle.Foes.Count);
			Assert.IsFalse(gold.Battle.Foes[0].Boss, "재화 던전에 구역 보스가 나왔다");
		}

		/// <summary>★ 시뮬 안: 보스를 잡으면 판이 끝나고 원래 구역 전장으로. 구역 셈은 안 움직임</summary>
		[Test]
		public void Sim_BossKill_EndsTheRun_AndReturnsToTheStage()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			state.Stage = 5;
			state.KillsInStage = 2;
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Boss));
			IdleBattleSim.Advance(state, tuning, 0.1d);
			state.Battle.Foes[0].Health = 0d;

			IdleBattleSim.Advance(state, tuning, 0.1d);

			Assert.IsFalse(state.Dungeon.Active);
			Assert.IsTrue(state.LastDungeonResult.Cleared);
			Assert.AreEqual(5, state.Stage);
			Assert.AreEqual(2, state.KillsInStage, "던전 처치가 구역 처치로 셌다");
			Assert.IsTrue(state.Battle.Ready);
			Assert.Greater(state.Battle.Foes.Count, 0, "원래 구역 웨이브가 안 섰다");
			Assert.IsFalse(state.Battle.Foes[0].Boss);
		}

		/// <summary>★ 전멸은 얻은 것만 들고 나감. 구역 후퇴 없음 (사용자 2026-09-08)</summary>
		[Test]
		public void Sim_Wipe_EndsTheRun_KeepsLoot_NoFallBack()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			state.Stage = 5;
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));
			IdleDungeons.OnKill(state, tuning, false);
			double loot = state.Resource;
			Assert.Greater(loot, 0d);

			for (int seat = 0; seat < IdleSquad.SEAT_COUNT; seat++)
			{
				// 쓰러진 자리 (부활 게이지가 돌기 시작한 것). 체력만 0 이면 EnsureSeatRoom 이 새 자리로 보고 채움
				state.SeatHealth[seat] = 0d;
				state.SeatReviveSeconds[seat] = 0.01d;
			}

			IdleBattleSim.Advance(state, tuning, 0.1d);

			Assert.IsFalse(state.Dungeon.Active);
			Assert.IsFalse(state.LastDungeonResult.Cleared);
			Assert.AreEqual(loot, state.LastDungeonResult.Gold, 1e-9d);
			Assert.AreEqual(loot, state.Resource, 1e-9d);
			Assert.AreEqual(5, state.Stage, "던전 전멸이 구역을 물렸다");
		}

		/// <summary>★ 소탕은 한 번 깬 던전만. 한 판은 풀 클리어 몫</summary>
		[Test]
		public void Sweep_NeedsAClear_ThenPaysFullClearsPerTicket()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			Assert.IsFalse(IdleDungeons.TrySweep(state, tuning, IdleDungeonKind.Gold, out IdleDungeonReward none));
			Assert.AreEqual(0, none.Runs);
			Assert.AreEqual(tuning.TicketsPerDay, IdleDungeons.TicketsOf(state, IdleDungeonKind.Gold), "안 깬 던전 소탕이 입장권을 썼다");

			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));
			IdleDungeons.TickRun(state, tuning, IdleDungeons.SpecOf(tuning, IdleDungeonKind.Gold).TimeLimitSeconds);
			double before = state.Resource;
			double perRun = IdleDungeons.SweepGoldOf(state, tuning, IdleDungeonKind.Gold);

			Assert.IsTrue(IdleDungeons.TrySweep(state, tuning, IdleDungeonKind.Gold, out IdleDungeonReward all));

			Assert.AreEqual(tuning.TicketsPerDay - 1L, (long)all.Runs);
			Assert.AreEqual(perRun * all.Runs, all.Gold, 1e-6d);
			Assert.AreEqual(before + all.Gold, state.Resource, 1e-6d);
			Assert.AreEqual(0L, IdleDungeons.TicketsOf(state, IdleDungeonKind.Gold));
			Assert.IsFalse(state.Dungeon.Active, "소탕이 판을 열었다");
		}

		[Test]
		public void Sweep_WhileRunning_OrWithNoTickets_DoesNothing()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			state.DungeonCleared = 1L << (int)IdleDungeonKind.Boss;

			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Boss));
			Assert.IsFalse(IdleDungeons.TrySweep(state, tuning, IdleDungeonKind.Boss, out IdleDungeonReward _), "판 안에서 소탕이 됐다");
			IdleDungeons.EndRun(state, tuning, false);

			Assert.IsTrue(IdleDungeons.TrySweep(state, tuning, IdleDungeonKind.Boss, out IdleDungeonReward all));
			Assert.AreEqual(tuning.TicketsPerDay - 1L, (long)all.Runs);
			Assert.IsFalse(IdleDungeons.TrySweep(state, tuning, IdleDungeonKind.Boss, out IdleDungeonReward again));
			Assert.AreEqual(0, again.Runs);
		}

		/// <summary>★ 깬 기록은 저장에 남고 판은 안 남음</summary>
		[Test]
		public void Save_KeepsClears_DropsTheRun()
		{
			IdleTuning tuning = new IdleTuning();
			IdleState state = Ready(tuning);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Gold));
			IdleDungeons.TickRun(state, tuning, 1e9d);
			Assert.IsTrue(IdleDungeons.TryStart(state, tuning, IdleDungeonKind.Boss));

			IdleState loaded = new IdleState();
			loaded.Load(state.Save());

			Assert.IsTrue(IdleDungeons.IsCleared(loaded, IdleDungeonKind.Gold));
			Assert.IsFalse(IdleDungeons.IsCleared(loaded, IdleDungeonKind.Boss));
			Assert.IsFalse(loaded.Dungeon.Active);
		}

		/// <summary>★ 자리를 비우면 판은 그 자리에서 끝. 사진에 결과가 실림</summary>
		[Test]
		public void Session_CatchUp_EndsTheRun_AndTheSnapshotCarriesTheResult()
		{
			IdleTuning tuning = new IdleTuning();
			IdleSession session = new IdleSession(tuning);
			const long NOON = 1_700_000_000L;
			session.CatchUp(NOON);
			Assert.IsTrue(session.TryEnterDungeon(IdleDungeonKind.Gold));
			Assert.IsTrue(session.Capture().DungeonRun.Active);

			session.CatchUp(NOON + 600L);
			IdleSnapshot snapshot = session.Capture();

			Assert.IsFalse(snapshot.DungeonRun.Active);
			Assert.AreEqual(1L, snapshot.DungeonResultSequence);
			Assert.IsFalse(snapshot.LastDungeonResult.Cleared);
			Assert.AreEqual(IdleDungeons.COUNT, snapshot.DungeonSpecs.Length);
			Assert.IsFalse(snapshot.DungeonSpecs[(int)IdleDungeonKind.Skill].Open);
			Assert.IsFalse(snapshot.DungeonSpecs[(int)IdleDungeonKind.Gold].Cleared);
		}
	}
}
