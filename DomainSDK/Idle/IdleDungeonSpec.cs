namespace WitchMendokusai.DomainSDK.Idle
{
    /// <summary>
    /// 던전 하나의 판 규칙 (알파 9번, changes/idle-dungeon-run). 값은 SO 가 준다 (사용자 2026-09-08: 하드코딩 금지)
    ///
    /// 종류마다 쓰는 칸이 다름. 재화는 시간과 처치당 골드, 보스는 시간과 조각과 장비, 장비는 웨이브 수와 웨이브당 장비
    /// 소탕 (한 번 깬 뒤 안 들어가고 받기) 은 풀 클리어 몫: 재화는 소탕 골드 초, 보스는 조각과 장비, 장비는 웨이브 x 장비
    /// </summary>
    public sealed class IdleDungeonSpec
    {
        public IdleDungeonKind Kind { get; set; }

        /// <summary>닫힌 던전은 입장도 소탕도 못 함 (스킬 던전은 스킬 재료가 아직 없음)</summary>
        public bool Open { get; set; } = true;

        /// <summary>시간 제한 (초). 0 이면 없음. 끝나면 판이 끝남 (재화는 그때가 클리어)</summary>
        public double TimeLimitSeconds { get; set; }

        /// <summary>장비 던전의 웨이브 수. 다 밀면 클리어. 0 이면 웨이브 조건 없음</summary>
        public int Waves { get; set; }

        /// <summary>재화 던전. 처치 하나가 주는 골드를 초당 수입 몇 초치로 셈</summary>
        public double GoldSecondsPerKill { get; set; }

        /// <summary>재화 던전 소탕 한 판이 주는 골드 (초당 수입 몇 초치)</summary>
        public double SweepGoldSeconds { get; set; }

        /// <summary>보스 던전. 보스를 잡으면 환생 조각</summary>
        public long Shards { get; set; }

        /// <summary>보스 던전은 잡았을 때, 장비 던전은 웨이브마다 주는 장비 수 (지금 구역의 최고 등급)</summary>
        public long GearCount { get; set; }

        /// <summary>코드 기본값. 실제 값은 SO (DungeonCatalogSO). 시험과 SO 가 비었을 때만</summary>
        public static IdleDungeonSpec[] Defaults()
        {
            return new[]
            {
                new IdleDungeonSpec { Kind = IdleDungeonKind.Gold, TimeLimitSeconds = 90d, GoldSecondsPerKill = 30d, SweepGoldSeconds = 900d },
                new IdleDungeonSpec { Kind = IdleDungeonKind.Boss, TimeLimitSeconds = 120d, Shards = 3L, GearCount = 2L },
                new IdleDungeonSpec { Kind = IdleDungeonKind.Gear, TimeLimitSeconds = 180d, Waves = 5, GearCount = 1L },
                new IdleDungeonSpec { Kind = IdleDungeonKind.Skill, Open = false },
            };
        }
    }

    /// <summary>던전 한 판이 살아 있는 동안의 상태. 저장 안 함 (얻는 즉시 상태에 들어가므로 되돌릴 것이 없음)</summary>
    public sealed class IdleDungeonRun
    {
        public bool Active { get; set; }

        public IdleDungeonKind Kind { get; set; }

        public double TimeLimitSeconds { get; set; }

        public double SecondsLeft { get; set; }

        public int Waves { get; set; }

        public int WavesCleared { get; set; }

        public long Kills { get; set; }

        public double Gold { get; set; }

        public long Shards { get; set; }

        public int Gear { get; set; }

        public void Clear()
        {
            Active = false;
            TimeLimitSeconds = 0d;
            SecondsLeft = 0d;
            Waves = 0;
            WavesCleared = 0;
            Kills = 0L;
            Gold = 0d;
            Shards = 0L;
            Gear = 0;
        }
    }

    /// <summary>끝난 던전 한 판의 결과. 화면이 팝업으로 적는다</summary>
    public readonly struct IdleDungeonResult
    {
        public IdleDungeonResult(IdleDungeonKind kind, bool cleared, double secondsSpent, long kills, int wavesCleared, double gold, long shards, int gear)
        {
            Kind = kind;
            Cleared = cleared;
            SecondsSpent = secondsSpent;
            Kills = kills;
            WavesCleared = wavesCleared;
            Gold = gold;
            Shards = shards;
            Gear = gear;
        }

        public IdleDungeonKind Kind { get; }

        /// <summary>끝까지 깼나. 재화는 시간을 버팀, 보스는 잡음, 장비는 웨이브 다 밈. 깬 던전만 소탕이 열림</summary>
        public bool Cleared { get; }

        public double SecondsSpent { get; }

        public long Kills { get; }

        public int WavesCleared { get; }

        public double Gold { get; }

        public long Shards { get; }

        /// <summary>가방에 실제로 들어간 장비 수</summary>
        public int Gear { get; }
    }

    /// <summary>사진에 실리는 판 상태</summary>
    public readonly struct IdleDungeonRunView
    {
        public IdleDungeonRunView(bool active, IdleDungeonKind kind, double secondsLeft, double timeLimitSeconds, int wavesCleared, int waves, long kills, double gold, long shards, int gear)
        {
            Active = active;
            Kind = kind;
            SecondsLeft = secondsLeft;
            TimeLimitSeconds = timeLimitSeconds;
            WavesCleared = wavesCleared;
            Waves = waves;
            Kills = kills;
            Gold = gold;
            Shards = shards;
            Gear = gear;
        }

        public bool Active { get; }

        public IdleDungeonKind Kind { get; }

        public double SecondsLeft { get; }

        public double TimeLimitSeconds { get; }

        public int WavesCleared { get; }

        public int Waves { get; }

        public long Kills { get; }

        public double Gold { get; }

        public long Shards { get; }

        public int Gear { get; }
    }

    /// <summary>사진에 실리는 던전 하나의 규칙과 소탕 몫. 화면이 줄에 적는다</summary>
    public readonly struct IdleDungeonSpecView
    {
        public IdleDungeonSpecView(IdleDungeonKind kind, bool open, bool cleared, double timeLimitSeconds, int waves, double sweepGold, long shards, long gearCount)
        {
            Kind = kind;
            Open = open;
            Cleared = cleared;
            TimeLimitSeconds = timeLimitSeconds;
            Waves = waves;
            SweepGold = sweepGold;
            Shards = shards;
            GearCount = gearCount;
        }

        public IdleDungeonKind Kind { get; }

        public bool Open { get; }

        /// <summary>한 번이라도 깼나. 소탕이 열리는 조건</summary>
        public bool Cleared { get; }

        public double TimeLimitSeconds { get; }

        public int Waves { get; }

        /// <summary>재화 던전 소탕 한 판의 골드 (지금 수입 기준)</summary>
        public double SweepGold { get; }

        public long Shards { get; }

        public long GearCount { get; }
    }
}
